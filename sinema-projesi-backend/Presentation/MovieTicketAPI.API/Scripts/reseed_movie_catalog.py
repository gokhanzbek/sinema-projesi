"""
Tüm filmleri/kategorileri güvenli sırayla siler (seans + bilet dahil), 20 gerçek film + kategorileri yükler.
Salonlar (Halls) korunur. API çalışırken: python reseed_movie_catalog.py

Varsayılan: http://localhost:5000/api — launchSettings'e göre portu değiştir.
"""
from __future__ import annotations

import json
import sys
import urllib.error
import urllib.request

BASE = "http://localhost:5000/api"


def main() -> None:
    if len(sys.argv) > 1:
        global BASE
        BASE = sys.argv[1].rstrip("/")
        if not BASE.endswith("/api"):
            BASE = BASE + "/api" if "/api" not in BASE else BASE

    login = urllib.request.Request(
        f"{BASE}/Users/Login",
        data=json.dumps(
            {"usernameOrEmail": "admin", "password": "Admin123*"}
        ).encode("utf-8"),
        headers={"Content-Type": "application/json; charset=utf-8"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(login) as r:
            body = json.loads(r.read().decode())
            tok = body["token"]["accessToken"]
    except urllib.error.HTTPError as e:
        print("Giriş başarısız:", e.read().decode())
        sys.exit(1)

    req = urllib.request.Request(
        f"{BASE}/AdminTools/reseed-movie-catalog",
        data=b"{}",
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Bearer {tok}",
        },
        method="POST",
    )
    try:
        with urllib.request.urlopen(req) as r:
            print(json.dumps(json.loads(r.read().decode()), indent=2, ensure_ascii=False))
    except urllib.error.HTTPError as e:
        print("Reseed hatası:", e.code, e.read().decode())
        sys.exit(1)


if __name__ == "__main__":
    main()
