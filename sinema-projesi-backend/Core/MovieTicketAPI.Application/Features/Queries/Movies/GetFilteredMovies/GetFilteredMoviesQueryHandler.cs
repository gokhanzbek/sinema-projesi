using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetFilteredMovies
{
    public class GetFilteredMoviesQueryHandler : IRequestHandler<GetFilteredMoviesQueryRequest, GetFilteredMoviesQueryResponse>
    {
        private readonly IShowTimeReadRepository _showTimeReadRepository;
        private readonly IMovieReadRepository _movieReadRepository;

        public GetFilteredMoviesQueryHandler(
            IShowTimeReadRepository showTimeReadRepository,
            IMovieReadRepository movieReadRepository)
        {
            _showTimeReadRepository = showTimeReadRepository;
            _movieReadRepository = movieReadRepository;
        }

        public async Task<GetFilteredMoviesQueryResponse> Handle(GetFilteredMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            var eventDate = (request.EventDate ?? DateTime.Today).Date;
            var dayStart = eventDate;
            var dayEnd = eventDate.AddDays(1);

            var q = _showTimeReadRepository.GetAll(false)
                .Where(s => s.StartTime >= dayStart && s.StartTime < dayEnd);

            if (request.MinPrice.HasValue)
                q = q.Where(s => s.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                q = q.Where(s => s.Price <= request.MaxPrice.Value);

            var slots = ParseTimeSlots(request.TimeSlots);
            if (slots.Count > 0)
            {
                var slot12_18 = slots.Contains("slot12_18");
                var slot18_22 = slots.Contains("slot18_22");
                var slot22_00 = slots.Contains("slot22_00");
                var slot00_12 = slots.Contains("slot00_12");
                q = q.Where(s =>
                    (slot12_18 && s.StartTime.Hour >= 12 && s.StartTime.Hour < 18) ||
                    (slot18_22 && s.StartTime.Hour >= 18 && s.StartTime.Hour < 22) ||
                    (slot22_00 && s.StartTime.Hour >= 22) ||
                    (slot00_12 && s.StartTime.Hour < 12));
            }

            var movieIds = await q.Select(s => s.MovieId).Distinct().ToListAsync(cancellationToken);

            if (movieIds.Count == 0)
            {
                return new GetFilteredMoviesQueryResponse { TotalCount = 0, Movies = Array.Empty<object>() };
            }

            var categoryNames = ParseGenres(request.Genres);
            var moviesQuery = _movieReadRepository.GetAll(false).Where(m => movieIds.Contains(m.Id));

            if (categoryNames.Count > 0)
            {
                moviesQuery = moviesQuery.Where(m =>
                    m.MovieCategories.Any(mc => categoryNames.Contains(mc.Category.Name)));
            }

            var movies = await moviesQuery
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .ToListAsync(cancellationToken);

            SortMovies(movies, request.Sort);

            var payload = movies.Select(m => new
            {
                m.Id,
                m.Title,
                m.ImageUrl,
                Categories = m.MovieCategories.Select(mc => mc.Category.Name).ToList(),
                ImdbRating = m.ImdbRating
            }).ToList();

            return new GetFilteredMoviesQueryResponse
            {
                TotalCount = payload.Count,
                Movies = payload
            };
        }

        private static List<string> ParseTimeSlots(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "slot12_18", "slot18_22", "slot22_00", "slot00_12"
            };

            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => allowed.Contains(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<string> ParseGenres(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(g => g.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void SortMovies(List<Movie> movies, string? sort)
        {
            switch ((sort ?? "recommended").Trim().ToLowerInvariant())
            {
                case "imdb_desc":
                    movies.Sort((a, b) => decimal.Compare(b.ImdbRating ?? 0, a.ImdbRating ?? 0));
                    break;
                case "imdb_asc":
                    movies.Sort((a, b) => decimal.Compare(a.ImdbRating ?? 0, b.ImdbRating ?? 0));
                    break;
                case "title_asc":
                    movies.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
                    break;
                case "title_desc":
                    movies.Sort((a, b) => string.Compare(b.Title, a.Title, StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    movies.Sort((a, b) => a.Id.CompareTo(b.Id));
                    break;
            }
        }
    }
}
