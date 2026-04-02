using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories.Categories;
using MovieTicketAPI.Application.Repositories.MovieCategories;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.CreateMovie
{
    public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommandRequest, CreateMovieCommandResponse>
    {
        private readonly IMovieWriteRepository _movieWriteRepository;
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly IMovieCategoryRepository _movieCategoryRepository;
        private readonly IOmdbMovieRatingService _omdbMovieRatingService;

        public CreateMovieCommandHandler(
            IMovieWriteRepository movieWriteRepository,
            ICategoryReadRepository categoryReadRepository,
            IMovieCategoryRepository movieCategoryRepository,
            IOmdbMovieRatingService omdbMovieRatingService)
        {
            _movieWriteRepository = movieWriteRepository;
            _categoryReadRepository = categoryReadRepository;
            _movieCategoryRepository = movieCategoryRepository;
            _omdbMovieRatingService = omdbMovieRatingService;
        }

        public async Task<CreateMovieCommandResponse> Handle(CreateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var ids = request.CategoryIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0)
            {
                return new CreateMovieCommandResponse
                {
                    IsSuccess = false,
                    Message = "En az bir kategori seçilmelidir."
                };
            }

            var foundCount = await _categoryReadRepository.GetWhere(c => ids.Contains(c.Id), false)
                .CountAsync(cancellationToken);
            if (foundCount != ids.Count)
            {
                return new CreateMovieCommandResponse
                {
                    IsSuccess = false,
                    Message = "Geçersiz kategori kimliği var."
                };
            }

            var imdbIdNorm = NormalizeImdbId(request.ImdbId);

            var imdbRating = await _omdbMovieRatingService.GetImdbRatingByTitleYearAsync(
                request.Title,
                request.ReleaseYear,
                imdbIdNorm,
                cancellationToken).ConfigureAwait(false);

            var newMovie = new MovieTicketAPI.Domain.Entities.Movie
            {
                ImageUrl = request.ImageUrl,
                Title = request.Title,
                DurationInMinutes = request.DurationInMinutes,
                Director = request.Director,
                ReleaseYear = request.ReleaseYear,
                Description = request.Description,
                ImdbId = imdbIdNorm,
                ImdbRating = imdbRating
            };
            await _movieWriteRepository.AddAsync(newMovie);
            await _movieWriteRepository.SaveAsync();

            await _movieCategoryRepository.SetCategoriesForMovieAsync(newMovie.Id, ids, cancellationToken);

            return new CreateMovieCommandResponse
            {
                Id = newMovie.Id,
                IsSuccess = true,
                Message = "Görev başarıyla eklendi!",
                ImdbRating = newMovie.ImdbRating
            };
        }

        private static string? NormalizeImdbId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var t = value.Trim();
            return t.Length > 20 ? t[..20] : t;
        }
    }
}
