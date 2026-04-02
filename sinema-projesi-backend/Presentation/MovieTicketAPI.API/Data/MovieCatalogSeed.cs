namespace MovieTicketAPI.API.Data;

/// <summary>
/// Reseed için ortak katalog (yaklaşık IMDb puanları; OMDb senkronu ile güncellenebilir).
/// </summary>
public static class MovieCatalogSeed
{
    public sealed record Entry(
        string Title,
        int DurationMinutes,
        int Year,
        string Director,
        string Description,
        string? ImdbId,
        decimal ImdbRating,
        string[] Categories,
        string PosterSlug);

    public static IReadOnlyList<Entry> Entries { get; } =
        new Entry[]
        {
            new("Kış Uykusu", 196, 2014, "Nuri Bilge Ceylan",
                "Emekli oyuncu Aydın'ın Kapadokya'daki kışı; aile ve iç hesaplaşma.", "tt2758880", 8.0m,
                new[] { "Dram", "Türk Sineması" }, "Kis Uykusu"),
            new("Ayla", 125, 2017, "Can Ulkay",
                "Kore Savaşı'nda bir Türk astsubay ile küçük kızın öyküsü.", "tt6319818", 8.2m,
                new[] { "Dram", "Macera", "Türk Sineması" }, "Ayla"),
            new("Eşkıya", 128, 1996, "Yavuz Turgul",
                "İstanbul ve Anadolu'da hesaplaşma; klasik Türk sineması.", "tt0116231", 8.2m,
                new[] { "Dram", "Suç", "Türk Sineması" }, "Eskiya"),
            new("Hababam Sınıfı", 87, 1975, "Ertem Eğilmez",
                "Özel dershanedeki öğrenci ve vekil sınıf komedisi.", "tt0252482", 8.8m,
                new[] { "Komedi", "Türk Sineması" }, "Hababam"),
            new("G.O.R.A.", 125, 2004, "Ömer Faruk Sorak",
                "Uzaylılara karşı bilim kurgu komedisi.", "tt0384116", 7.8m,
                new[] { "Komedi", "Bilim Kurgu", "Türk Sineması" }, "GORA"),
            new("Nefes: Vatan Sağolsun", 128, 2009, "Levent Semerci",
                "Dağdaki bir karakolun gerilim dolu mücadelesi.", "tt1781827", 8.0m,
                new[] { "Dram", "Gerilim", "Türk Sineması" }, "Nefes"),
            new("7. Koğuştaki Mucize", 132, 2019, "Mehmet Ada Öztekin",
                "Baba-kız ilişkisi ve adalet temalı dram.", "tt10439366", 8.2m,
                new[] { "Dram", "Türk Sineması" }, "Mucize"),
            new("Düğün Dernek", 102, 2013, "Selçuk Aydemir",
                "Sıra dışı düğün hazırlıkları komedisi.", "tt3398048", 6.8m,
                new[] { "Komedi", "Türk Sineması" }, "Dugun Dernek"),
            new("Cep Herkülü: Naim Süleymanoğlu", 141, 2019, "Özer Feyzioğlu",
                "Olimpiyat şampiyonu haltercinin biyografik öyküsü.", "tt9899868", 7.4m,
                new[] { "Dram", "Tarih", "Türk Sineması" }, "Naim"),
            new("Inception", 148, 2010, "Christopher Nolan",
                "Rüya katmanlarında zihin hırsızlığı.", "tt1375666", 8.8m,
                new[] { "Bilim Kurgu", "Gerilim", "Aksiyon" }, "Inception"),
            new("Interstellar", 169, 2014, "Christopher Nolan",
                "Solucan deliği ve yeni dünyalar arayışı.", "tt0816692", 8.7m,
                new[] { "Bilim Kurgu", "Dram", "Macera" }, "Interstellar"),
            new("Dune: Çöl Gezegeni", 155, 2021, "Denis Villeneuve",
                "Arrakis'te baharat ve kader savaşı.", "tt1160419", 8.0m,
                new[] { "Bilim Kurgu", "Macera", "Dram" }, "Dune"),
            new("Oppenheimer", 180, 2023, "Christopher Nolan",
                "Atom bombasının bilimsel ve etik tarihi.", "tt15398776", 8.4m,
                new[] { "Dram", "Tarih" }, "Oppenheimer"),
            new("Spider-Man: Örümcek Evreninde", 117, 2018, "Bob Persichetti, Peter Ramsey, Rodney Rothman",
                "Çoklu evrende Miles Morales.", "tt4633694", 8.4m,
                new[] { "Animasyon", "Aksiyon", "Macera" }, "SpiderVerse"),
            new("Whiplash", 106, 2014, "Damien Chazelle",
                "Caz davulunda mükemmellik takıntısı.", "tt2582802", 8.5m,
                new[] { "Dram", "Gerilim" }, "Whiplash"),
            new("Joker", 122, 2019, "Todd Phillips",
                "Gotham'da Arthur Fleck'in dönüşümü.", "tt7286456", 8.4m,
                new[] { "Dram", "Suç", "Gerilim" }, "Joker"),
            new("Yüzüklerin Efendisi: Yüzük Kardeşliği", 178, 2001, "Peter Jackson",
                "Orta Dünya'da yüzüğü yok etme yolculuğu.", "tt0120737", 8.9m,
                new[] { "Fantastik", "Macera", "Dram" }, "LOTR"),
            new("Matrix", 136, 1999, "Lana Wachowski, Lilly Wachowski",
                "Simülasyon ve gerçeklik sorusu.", "tt0133093", 8.7m,
                new[] { "Bilim Kurgu", "Aksiyon" }, "Matrix"),
            new("Gladyatör", 155, 2000, "Ridley Scott",
                "Roma'da arenada intikam.", "tt0172495", 8.5m,
                new[] { "Aksiyon", "Dram", "Tarih" }, "Gladiator"),
            new("Parazit", 132, 2019, "Bong Joon-ho",
                "Sınıf farklarının gerilimi.", "tt6751668", 8.5m,
                new[] { "Dram", "Gerilim", "Komedi" }, "Parasite")
        };
}
