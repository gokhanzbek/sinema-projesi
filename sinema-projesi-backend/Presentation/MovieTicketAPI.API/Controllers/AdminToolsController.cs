using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.API.Data;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;

namespace MovieTicketAPI.API.Controllers;

/// <summary>
/// Geliştirme / yönetim: katalog toptan yenileme. Salonlar (Halls) dokunulmaz; eski filme bağlı seans ve biletler silinir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminToolsController : ControllerBase
{
    private readonly MovieTicketDbContext _db;

    public AdminToolsController(MovieTicketDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Tüm biletleri, seansları, film–kategori eşleşmelerini, filmleri ve kategorileri siler; ardından örnek kategori + 20 gerçek film ekler.
    /// </summary>
    [HttpPost("reseed-movie-catalog")]
    public async Task<IActionResult> ReseedMovieCatalog(CancellationToken cancellationToken)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Tickets.RemoveRange(await _db.Tickets.ToListAsync(cancellationToken));
        _db.Showtimes.RemoveRange(await _db.Showtimes.ToListAsync(cancellationToken));
        _db.MovieCategories.RemoveRange(await _db.MovieCategories.ToListAsync(cancellationToken));
        _db.Movies.RemoveRange(await _db.Movies.ToListAsync(cancellationToken));
        _db.Categories.RemoveRange(await _db.Categories.ToListAsync(cancellationToken));
        await _db.SaveChangesAsync(cancellationToken);

        var categoryNames = new[]
        {
            "Dram", "Komedi", "Aksiyon", "Bilim Kurgu", "Korku", "Animasyon", "Belgesel",
            "Romantik", "Gerilim", "Macera", "Tarih", "Suç", "Fantastik", "Türk Sineması"
        };

        foreach (var name in categoryNames)
            _db.Categories.Add(new Category { Name = name });

        await _db.SaveChangesAsync(cancellationToken);

        var catId = await _db.Categories
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Name, c => c.Id, cancellationToken);

        static string Poster(string slug) =>
            $"https://placehold.co/400x600/0f172a/cbd5e1/png?text={Uri.EscapeDataString(slug)}";

        var specs = MovieCatalogSeed.Entries;

        var movies = new List<Movie>();
        foreach (var s in specs)
        {
            movies.Add(new Movie
            {
                Title = s.Title,
                DurationInMinutes = s.DurationMinutes,
                ReleaseYear = s.Year,
                Director = s.Director.Length > 100 ? s.Director[..100] : s.Director,
                Description = s.Description.Length > 500 ? s.Description[..500] : s.Description,
                ImageUrl = Poster(s.PosterSlug),
                ImdbId = s.ImdbId,
                ImdbRating = s.ImdbRating
            });
        }

        _db.Movies.AddRange(movies);
        await _db.SaveChangesAsync(cancellationToken);

        for (var i = 0; i < movies.Count; i++)
        {
            var movieId = movies[i].Id;
            foreach (var c in specs[i].Categories)
            {
                if (!catId.TryGetValue(c, out var categoryId))
                    continue;
                _db.MovieCategories.Add(new MovieCategory { MovieId = movieId, CategoryId = categoryId });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return Ok(new
        {
            message = "Katalog yenilendi. Eski seans ve biletler silindi; salonlar korundu. Yeni seansları admin panelden ekleyin.",
            categories = categoryNames.Length,
            movies = specs.Count
        });
    }
}
