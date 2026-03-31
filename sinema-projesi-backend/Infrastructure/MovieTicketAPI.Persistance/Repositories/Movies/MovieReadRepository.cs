using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Repositories.Movies
{
    public class MovieReadRepository : ReadRepository<Movie>, IMovieReadRepository
    {
        public MovieReadRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
