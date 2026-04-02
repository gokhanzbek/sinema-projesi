"""
Günlük sabit planı 10 gün boyunca tekrarlar (7 salon × 3 zaman dilimi = 21 seans/gün).

Plan (F1-F20 = veritabaninda Id'ye gore sirali ilk 20 film):
  12:00-18:00: Salon 1..7 -> F1..F7   (baslangic: 15:00)
  18:00-20:00: Salon 1..7 -> F1, F8..F13 (baslangic: 19:00)
  20:00-00:00: Salon 1..7 -> F14..F20 (baslangic: 21:30)

Önce tüm seansları siler, sonra Admin ile ekler.

  python seed_showtimes_10day_plan.py
  set MOVIE_API_BASE=http://localhost:5000/api
  set PLAN_START_DATE=2026-04-01   (isteğe bağlı; yoksa bugün)
"""
from __future__ import annotations

import json
import os
import sys
import urllib.error
import urllib.request
from datetime import date, datetime, timedelta

BASE = os.environ.get("MOVIE_API_BASE", "http://localhost:5000/api").rstrip("/")
PLAN_DAYS = 10
REQUIRED_MOVIES = 20
REQUIRED_HALLS = 7

# Her dilimin içinde tek başlangıç saati (salon başına bir seans)
SLOT_TIMES = (
    (15, 0),   # öğleden sonra bloğu
    (19, 0),   # akşam erken
    (21, 30),  # gece
)

# Her dilimde salon sırasına göre film indeksi (0 tabanlı: F1 -> 0)
SLOT_FILM_INDICES = (
    (0, 1, 2, 3, 4, 5, 6),      # F1..F7
    (0, 7, 8, 9, 10, 11, 12),   # F1, F8..F13
    (13, 14, 15, 16, 17, 18, 19),  # F14..F20
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


def fetch_halls(token: str) -> list[dict]:
    _, raw = req("GET", "/Halls", token=token)
    hj = json.loads(raw.decode())
    halls = hj.get("halls") or hj.get("Halls") or []
    return sorted(halls, key=lambda x: int(x.get("id") or x.get("Id")))


def ensure_hall_count(token: str, halls: list[dict], need: int) -> list[dict]:
    if len(halls) >= need:
        return halls[:need]
    next_num = len(halls) + 1
    while len(halls) < need:
        name = f"Salon {next_num}"
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
        halls = fetch_halls(token)
    return sorted(halls, key=lambda x: int(x.get("id") or x.get("Id")))[:need]


def main() -> None:
    token = login()
    delete_all_showtimes(token)

    _, raw = req("GET", "/Movies")
    mj = json.loads(raw.decode())
    movies = mj.get("movies") or mj.get("Movies") or []
    movies = sorted(movies, key=lambda x: int(x.get("id") or x.get("Id")))
    mids = [int(m.get("id") or m.get("Id")) for m in movies]

    if len(mids) < REQUIRED_MOVIES:
        print(
            f"En az {REQUIRED_MOVIES} film gerekli (F1–F20); şu an {len(mids)} film var.",
            file=sys.stderr,
        )
        sys.exit(1)

    film_ids = mids[:REQUIRED_MOVIES]

    halls = fetch_halls(token)
    halls = ensure_hall_count(token, halls, REQUIRED_HALLS)
    hall_ids = [int(h.get("id") or h.get("Id")) for h in halls]

    start_raw = os.environ.get("PLAN_START_DATE", "").strip()
    if start_raw:
        y, m, d = map(int, start_raw.split("-", 2))
        start_date = date(y, m, d)
    else:
        start_date = date.today()

    created = 0
    for day_i in range(PLAN_DAYS):
        d = start_date + timedelta(days=day_i)
        for slot_i, ((hh, mm), film_ix_row) in enumerate(zip(SLOT_TIMES, SLOT_FILM_INDICES)):
            for hall_i, film_ix in enumerate(film_ix_row):
                mid = film_ids[film_ix]
                hid = hall_ids[hall_i]
                start_dt = datetime(d.year, d.month, d.day, hh, mm, 0)
                price = 150 + (mid % 13) * 10
                if price > 260:
                    price = 260
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

    print(
        f"Tamam: {created} seans ({PLAN_DAYS} gün × 21), "
        f"ilk gün {start_date.isoformat()}, salon Id'leri {hall_ids}, "
        f"film Id'leri F1..F20 -> {film_ids}"
    )


if __name__ == "__main__":
    main()
