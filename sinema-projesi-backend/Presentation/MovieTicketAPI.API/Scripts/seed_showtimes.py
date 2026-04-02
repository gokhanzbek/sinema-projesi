"""
Mevcut filmlere göre 7 günlük seans oluşturur.
- Varsayılan: her film günde 2 kez.
- Öne çıkan filmler (başlıkta anahtar kelime): günde 3 kez (ör. Inception).
- Salon sayısı, bir gün içindeki toplam seansı karşılamıyorsa otomatik yeni salon eklenir.

Önce tüm seansları siler, sonra Admin ile ekler.

  python seed_showtimes.py
  set MOVIE_API_BASE=http://localhost:5000/api
"""
from __future__ import annotations

import json
import os
import sys
import urllib.error
import urllib.request
from datetime import date, datetime, timedelta

BASE = os.environ.get("MOVIE_API_BASE", "http://localhost:5000/api").rstrip("/")

VIZYON_DAYS = 7
# 10:00–22:00 arası en fazla kaç dalga (90 dk aralık)
MAX_WAVES_PER_DAY = 8
WAVE_GAP_MINUTES = 90
DAY_START_MINUTES = 10 * 60  # 10:00

# Bu alt dizelerden biri başlıkta geçerse günde 3 seans (büyük/küçük harf duyarsız)
THREE_A_DAY_KEYWORDS = (
    "inception",
    "matrix",
    "joker",
    "yüzük",
    "yuzuk",
    "parazit",
    "baba",
    "interstellar",
    "yildizlararasi",
    "yıldız",
)


def req(
    method: str,
    path: str,
    *,
    body: dict | None = None,
    token: str | None = None,
) -> tuple[int, bytes]:
    url = f"{BASE}{path}" if path.startswith("/") else f"{BASE}/{path}"
    headers = {"Content-Type": "application/json; charset=utf-8"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    data = None
    if body is not None:
        data = json.dumps(body, ensure_ascii=False).encode("utf-8")
    r = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(r) as resp:
            return resp.status, resp.read()
    except urllib.error.HTTPError as e:
        return e.code, e.read()


def login() -> str:
    _, raw = req(
        "POST",
        "/Users/Login",
        body={"usernameOrEmail": "admin", "password": "Admin123*"},
    )
    d = json.loads(raw.decode())
    tok = d.get("token", {}).get("accessToken")
    if not tok:
        print("Giriş başarısız:", d, file=sys.stderr)
        sys.exit(1)
    return tok


def delete_all_showtimes(token: str) -> None:
    code, raw = req("GET", "/ShowTimes", token=token)
    if code != 200:
        print("ShowTimes listelenemedi:", code, raw.decode(), file=sys.stderr)
        sys.exit(1)
    data = json.loads(raw.decode())
    rows = data.get("showTimes") or data.get("ShowTimes") or []
    for row in rows:
        sid = row.get("id") or row.get("Id")
        if sid is None:
            continue
        c, _ = req("DELETE", f"/ShowTimes/{sid}", token=token)
        if c not in (200, 204):
            print(f"Silinemedi seans {sid}: {c}", file=sys.stderr)


def showings_per_movie(title: str) -> int:
    t = (title or "").lower()
    for kw in THREE_A_DAY_KEYWORDS:
        if kw in t:
            return 3
    return 2


def fetch_halls(token: str) -> list[dict]:
    _, raw = req("GET", "/Halls", token=token)
    hj = json.loads(raw.decode())
    halls = hj.get("halls") or hj.get("Halls") or []
    return sorted(halls, key=lambda x: int(x.get("id") or x.get("Id")))


def ensure_hall_capacity(token: str, halls: list[dict], total_showings_one_day: int) -> list[dict]:
    """Bir gün içinde MAX_WAVES_PER_DAY dalgada sığacak kadar salon olmasını sağlar."""
    need = (total_showings_one_day + MAX_WAVES_PER_DAY - 1) // MAX_WAVES_PER_DAY
    need = max(need, 1)
    existing = len(halls)
    if existing >= need:
        return halls

    to_create = need - existing
    next_num = existing + 1
    for _ in range(to_create):
        name = f"Salon-Oto-{next_num}"
        code, out = req(
            "POST",
            "/Halls",
            body={"name": name[:50], "rowCount": 10, "columnCount": 12},
            token=token,
        )
        if code != 200:
            print(f"Salon eklenemedi {name}: {code} {out.decode()}", file=sys.stderr)
            sys.exit(1)
        next_num += 1

    return fetch_halls(token)


def main() -> None:
    token = login()
    delete_all_showtimes(token)

    _, raw = req("GET", "/Movies")
    mj = json.loads(raw.decode())
    movies = mj.get("movies") or mj.get("Movies") or []
    movies = sorted(movies, key=lambda x: int(x.get("id") or x.get("Id")))

    if not movies:
        print("Film yok.", file=sys.stderr)
        sys.exit(1)

    per_movie = {
        int(m.get("id") or m.get("Id")): showings_per_movie(
            str(m.get("title") or m.get("Title") or "")
        )
        for m in movies
    }
    total_daily = sum(per_movie.values())

    halls = fetch_halls(token)
    halls = ensure_hall_capacity(token, halls, total_daily)
    nh = len(halls)

    waves_used = (total_daily + nh - 1) // nh
    if waves_used > MAX_WAVES_PER_DAY:
        print(
            f"Hata: Günlük {total_daily} seans için en az "
            f"{(total_daily + MAX_WAVES_PER_DAY - 1) // MAX_WAVES_PER_DAY} salon gerekir; "
            f"dalga sayısı {waves_used} > {MAX_WAVES_PER_DAY}. "
            f"THREE_A_DAY_KEYWORDS veya MAX_WAVES_PER_DAY ayarlayın.",
            file=sys.stderr,
        )
        sys.exit(1)

    start_date = date.today()
    created = 0

    for day_i in range(VIZYON_DAYS):
        d = start_date + timedelta(days=day_i)
        # Günün iş listesi: her gösterim için movie id
        jobs: list[int] = []
        for m in movies:
            mid = int(m.get("id") or m.get("Id"))
            for _ in range(per_movie[mid]):
                jobs.append(mid)

        for idx, mid in enumerate(jobs):
            wave = idx // nh
            hi = idx % nh
            hid = int(halls[hi].get("id") or halls[hi].get("Id"))
            minutes_from_midnight = DAY_START_MINUTES + wave * WAVE_GAP_MINUTES
            h, mm = divmod(minutes_from_midnight, 60)
            start_dt = datetime(d.year, d.month, d.day, h, mm, 0)
            price = 120 + (mid % 17) * 10
            if price > 280:
                price = 280
            body = {
                "startTime": start_dt.isoformat(),
                "price": float(price),
                "movieId": mid,
                "hallId": hid,
            }
            code, out = req("POST", "/ShowTimes", body=body, token=token)
            if code != 200:
                print(f"HATA {code} {body}: {out.decode()}", file=sys.stderr)
                sys.exit(1)
            created += 1

    three = [mid for mid, c in per_movie.items() if c == 3]
    print(
        f"Tamam: {created} seans ({VIZYON_DAYS} gün, günde toplam {total_daily} gösterim), "
        f"{nh} salon, başlangıç {start_date.isoformat()}.\n"
        f"Günde 3 kez: {len(three)} film id: {three[:15]}{'...' if len(three) > 15 else ''}"
    )


if __name__ == "__main__":
    main()
