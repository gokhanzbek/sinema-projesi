using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetAllMovies
{
    public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQueryRequest, GetAllMoviesQueryResponse>
    {
        private readonly IMovieReadRepository _movieReadRepository;
        public GetAllMoviesQueryHandler(IMovieReadRepository movieReadRepository) => _movieReadRepository = movieReadRepository;

        public async Task<GetAllMoviesQueryResponse> Handle(GetAllMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _movieReadRepository.GetAll(tracking: false);
            return new GetAllMoviesQueryResponse
            {
                TotalCount = await query.CountAsync(),
                Movies = await query.Select(m => new { m.Id, m.Title, m.ImageUrl }).ToListAsync() // ImageUrl'i unutmadık! 😎
            };
        }
    }
}
