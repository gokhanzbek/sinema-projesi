using MediatR;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie
{
    public class GetByIdMovieQueryHandler : IRequestHandler<GetByIdMovieQueryRequest, GetByIdMovieQueryResponse>
    {
        private readonly IMovieReadRepository _movieReadRepository;
        public GetByIdMovieQueryHandler(IMovieReadRepository movieReadRepository) => _movieReadRepository = movieReadRepository;

        public async Task<GetByIdMovieQueryResponse> Handle(GetByIdMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await _movieReadRepository.GetByIdAsync(request.Id.ToString(), tracking: false);
            if (movie == null) return new GetByIdMovieQueryResponse();

            return new GetByIdMovieQueryResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                ImageUrl = movie.ImageUrl,
                Description = movie.Description,
                DurationInMinutes = movie.DurationInMinutes,
                Genre = movie.Genre,
                Director = movie.Director,
                ReleaseYear = movie.ReleaseYear
            };
        }
    }
}
