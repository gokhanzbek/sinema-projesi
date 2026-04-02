using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie
{
    public class GetByIdMovieQueryHandler : IRequestHandler<GetByIdMovieQueryRequest, GetByIdMovieQueryResponse>
    {
        private readonly IMovieReadRepository _movieReadRepository;

        public GetByIdMovieQueryHandler(IMovieReadRepository movieReadRepository) => _movieReadRepository = movieReadRepository;

        public async Task<GetByIdMovieQueryResponse> Handle(GetByIdMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await _movieReadRepository.GetAll(false)
                .Where(m => m.Id == request.Id)
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .FirstOrDefaultAsync(cancellationToken);

            if (movie == null)
            {
                return new GetByIdMovieQueryResponse();
            }

            return new GetByIdMovieQueryResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                ImageUrl = movie.ImageUrl,
                Description = movie.Description,
                DurationInMinutes = movie.DurationInMinutes,
                Categories = movie.MovieCategories.Select(mc => mc.Category.Name).ToList(),
                CategoryIds = movie.MovieCategories.Select(mc => mc.CategoryId).ToList(),
                Director = movie.Director,
                ReleaseYear = movie.ReleaseYear,
                ImdbId = movie.ImdbId,
                ImdbRating = movie.ImdbRating
            };
        }
    }
}
