"""One-off: login + PUT movies 29-48 with proper Turkish titles. Run: python seed_real_movies_utf8.py"""
import json
import urllib.error
import urllib.request

BASE = "http://localhost:5000/api"


def main() -> None:
    login = urllib.request.Request(
        f"{BASE}/Users/Login",
        data=json.dumps(
            {"usernameOrEmail": "admin", "password": "Admin123*"}
        ).encode("utf-8"),
        headers={"Content-Type": "application/json; charset=utf-8"},
        method="POST",
    )
    with urllib.request.urlopen(login) as r:
        tok = json.loads(r.read().decode())["token"]["accessToken"]

    updates = [
        (
            29,
            "Yüzüklerin Efendisi: Yüzük Kardeşliği",
            [9, 14],
            178,
            2001,
            "Peter Jackson",
            "Orta Dünya'da epik bir macera. Fantastik ve Macera.",
        ),
        (
            30,
            "Inception",
            [6, 10],
            148,
            2010,
            "Christopher Nolan",
            "Rüya içinde rüya. Bilim Kurgu ve Gerilim.",
        ),
        (
            31,
            "Esaretin Bedeli",
            [8],
            142,
            1994,
            "Frank Darabont",
            "Umut ve dostluk hikâyesi. Klasik dram.",
        ),
        (
            32,
            "Baba",
            [8, 17],
            175,
            1972,
            "Francis Ford Coppola",
            "Corleone ailesi. Dram ve Suç.",
        ),
        (
            33,
            "Karayip Korsanları: Lanetli İnci'nin Gizemi",
            [14, 4],
            143,
            2003,
            "Gore Verbinski",
            "Jack Sparrow'un macerası. Macera ve Aksiyon.",
        ),
        (
            34,
            "Toy Story",
            [5, 3, 12],
            81,
            1995,
            "John Lasseter",
            "Oyuncakların dünyası. Animasyon, Aile ve Komedi.",
        ),
        (
            35,
            "Gladyatör",
            [4, 8],
            155,
            2000,
            "Ridley Scott",
            "Antik Roma'da intikam. Aksiyon ve Dram.",
        ),
        (
            36,
            "Sessiz Bir Yer",
            [13, 10],
            90,
            2018,
            "John Krasinski",
            "Sessizlik hayatta kalmak demek. Korku ve Gerilim.",
        ),
        (
            37,
            "Parazit",
            [8, 10, 11],
            132,
            2019,
            "Bong Joon-ho",
            "Sınıf çatışması. Dram, Gerilim ve Gizem.",
        ),
        (
            38,
            "Amélie'nin Muhteşem Hayatı",
            [15, 12],
            122,
            2001,
            "Jean-Pierre Jeunet",
            "Paris'te renkli bir kız. Romantik ve Komedi.",
        ),
        (
            39,
            "Rocky",
            [16, 8],
            120,
            1976,
            "John G. Avildsen",
            "Ringde yükseliş. Spor ve Dram.",
        ),
        (
            40,
            "Argo",
            [10, 8, 7],
            120,
            2012,
            "Ben Affleck",
            "Tahran'da tahliye operasyonu. Gerilim, Dram ve Biyografi.",
        ),
        (
            41,
            "Matrix",
            [6, 4],
            136,
            1999,
            "Lana ve Lilly Wachowski",
            "Gerçeklik sorgulanıyor. Bilim Kurgu ve Aksiyon.",
        ),
        (
            42,
            "Joker",
            [8, 17, 10],
            122,
            2019,
            "Todd Phillips",
            "Arthur Fleck'in dönüşümü. Dram, Suç ve Gerilim.",
        ),
        (
            43,
            "Coco",
            [5, 3],
            105,
            2017,
            "Lee Unkrich",
            "Día de Muertos ve müzik. Animasyon ve Aile.",
        ),
        (
            44,
            "Guguk Kuşu",
            [8],
            133,
            1975,
            "Milos Forman",
            "Akıl hastanesinde isyan. Dram.",
        ),
        (
            45,
            "Yeşil Yol",
            [8, 9],
            189,
            1999,
            "Frank Darabont",
            "Hapishane koridorlarında mucize. Dram ve fantastik öğeler.",
        ),
        (
            46,
            "Whiplash",
            [8],
            107,
            2014,
            "Damien Chazelle",
            "Müzik ve takıntı. Dram.",
        ),
        (
            47,
            "Yıldızlararası",
            [6, 8],
            169,
            2014,
            "Christopher Nolan",
            "Solucan deliği ve aile. Bilim Kurgu ve Dram.",
        ),
        (
            48,
            "Ucuz Roman",
            [17, 8, 12],
            154,
            1994,
            "Quentin Tarantino",
            "Los Angeles'ta örülen hikâyeler. Suç, Dram ve Komedi.",
        ),
    ]

    headers = {
        "Content-Type": "application/json; charset=utf-8",
        "Authorization": f"Bearer {tok}",
    }

    for mid, title, cats, dur, year, director, desc in updates:
        body = {
            "id": mid,
            "title": title,
            "durationInMinutes": dur,
            "categoryIds": cats,
            "director": director,
            "releaseYear": year,
            "description": desc,
            "imageUrl": "",
            "isCompleted": False,
        }
        payload = json.dumps(body, ensure_ascii=False).encode("utf-8")
        req = urllib.request.Request(
            f"{BASE}/Movies",
            data=payload,
            headers=headers,
            method="PUT",
        )
        try:
            with urllib.request.urlopen(req) as r:
                out = json.loads(r.read().decode())
            ok = out.get("isSuccess") or out.get("IsSuccess")
            print(f"OK {mid} {title[:50]}... success={ok}")
        except urllib.error.HTTPError as e:
            print(f"FAIL {mid} {title}: {e.code} {e.read().decode()}")


if __name__ == "__main__":
    main()
