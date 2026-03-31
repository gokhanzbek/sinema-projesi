using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Repositories.Movies
{
    public class MovieWriteRepository : WriteRepository<Movie>, IMovieWriteRepository
    {
        public MovieWriteRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
