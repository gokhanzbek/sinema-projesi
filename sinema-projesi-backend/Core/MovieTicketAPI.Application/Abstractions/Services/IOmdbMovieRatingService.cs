using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Abstractions.Services
{
    /// <summary>Harici OMDb kaynağından IMDb puanı alır (film oluşturma ve toplu senkron).</summary>
    public interface IOmdbMovieRatingService
    {
        Task<decimal?> GetImdbRatingByTitleYearAsync(
            string title,
            int releaseYear,
            string? imdbId = null,
            CancellationToken cancellationToken = default);
    }
}
