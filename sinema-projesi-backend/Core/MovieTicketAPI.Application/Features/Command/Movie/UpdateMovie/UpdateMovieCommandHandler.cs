using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Categories;
using MovieTicketAPI.Application.Repositories.MovieCategories;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie
{
    public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommandRequest, UpdateMovieCommandResponse>
    {
        private readonly IMovieReadRepository _movieReadRepository;
        private readonly IMovieWriteRepository _movieWriteRepository;
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly IMovieCategoryRepository _movieCategoryRepository;

        public UpdateMovieCommandHandler(
            IMovieReadRepository movieReadRepository,
            IMovieWriteRepository movieWriteRepository,
            ICategoryReadRepository categoryReadRepository,
            IMovieCategoryRepository movieCategoryRepository)
        {
            _movieReadRepository = movieReadRepository;
            _movieWriteRepository = movieWriteRepository;
            _categoryReadRepository = categoryReadRepository;
            _movieCategoryRepository = movieCategoryRepository;
        }

        async Task<UpdateMovieCommandResponse> IRequestHandler<UpdateMovieCommandRequest, UpdateMovieCommandResponse>.Handle(UpdateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var ids = request.CategoryIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0)
            {
                return new UpdateMovieCommandResponse { IsSuccess = false, Message = "En az bir kategori seçilmelidir." };
            }

            var foundCount = await _categoryReadRepository.GetWhere(c => ids.Contains(c.Id), false)
                .CountAsync(cancellationToken);
            if (foundCount != ids.Count)
            {
                return new UpdateMovieCommandResponse { IsSuccess = false, Message = "Geçersiz kategori kimliği var." };
            }

            var movie = await _movieReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);

            if (movie == null)
            {
                return new UpdateMovieCommandResponse { IsSuccess = false, Message = "Aradığınız Film bulunamadı!" };
            }

            movie.ImageUrl = request.ImageUrl;
            movie.Title = request.Title;
            movie.Director = request.Director;
            movie.DurationInMinutes = request.DurationInMinutes;
            movie.Description = request.Description;
            movie.ReleaseYear = request.ReleaseYear;
            movie.ImdbId = NormalizeImdbId(request.ImdbId);

            _movieWriteRepository.Update(movie);
            await _movieWriteRepository.SaveAsync();

            await _movieCategoryRepository.SetCategoriesForMovieAsync(movie.Id, ids, cancellationToken);

            return new UpdateMovieCommandResponse { IsSuccess = true, Message = "Film Başarıyla Güncellendi!" };
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
