using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.SyncMoviesImdbRatings
{
    public class SyncMoviesImdbRatingsCommandHandler
        : IRequestHandler<SyncMoviesImdbRatingsCommandRequest, SyncMoviesImdbRatingsCommandResponse>
    {
        private const int DelayBetweenOmdbCallsMs = 200;

        private readonly IMovieReadRepository _movieReadRepository;
        private readonly IMovieWriteRepository _movieWriteRepository;
        private readonly IOmdbMovieRatingService _omdbMovieRatingService;

        public SyncMoviesImdbRatingsCommandHandler(
            IMovieReadRepository movieReadRepository,
            IMovieWriteRepository movieWriteRepository,
            IOmdbMovieRatingService omdbMovieRatingService)
        {
            _movieReadRepository = movieReadRepository;
            _movieWriteRepository = movieWriteRepository;
            _omdbMovieRatingService = omdbMovieRatingService;
        }

        public async Task<SyncMoviesImdbRatingsCommandResponse> Handle(
            SyncMoviesImdbRatingsCommandRequest request,
            CancellationToken cancellationToken)
        {
            var movies = await _movieReadRepository
                .GetAll(tracking: true)
                .OrderBy(m => m.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var response = new SyncMoviesImdbRatingsCommandResponse
            {
                TotalMovies = movies.Count,
                IsSuccess = true,
                Message = "Senkronizasyon tamamlandı."
            };

            if (movies.Count == 0)
            {
                response.Message = "Güncellenecek film yok.";
                return response;
            }

            var index = 0;
            foreach (var movie in movies)
            {
                if (index > 0)
                {
                    await Task.Delay(DelayBetweenOmdbCallsMs, cancellationToken).ConfigureAwait(false);
                }

                index++;

                try
                {
                    var rating = await _omdbMovieRatingService
                        .GetImdbRatingByTitleYearAsync(movie.Title, movie.ReleaseYear, movie.ImdbId, cancellationToken)
                        .ConfigureAwait(false);

                    if (rating.HasValue)
                    {
                        movie.ImdbRating = rating;
                        _movieWriteRepository.Update(movie);
                        response.UpdatedCount++;
                    }
                    else
                    {
                        response.SkippedNoRatingCount++;
                    }
                }
                catch
                {
                    response.FailedCount++;
                }
            }

            await _movieWriteRepository.SaveAsync().ConfigureAwait(false);

            if (response.UpdatedCount == 0 && response.SkippedNoRatingCount == movies.Count && movies.Count > 0)
            {
                response.Message +=
                    " Hiç puan alınamadı: OMDb:ApiKey boş/geçersiz olabilir veya ağ engellidir. API konsol günlüğünde OMDb uyarılarına bakın.";
            }

            if (response.FailedCount > 0 && response.UpdatedCount == 0 && response.SkippedNoRatingCount == 0)
            {
                response.IsSuccess = false;
                response.Message = "Senkronizasyon sırasında hatalar oluştu.";
            }

            return response;
        }
    }
}
