using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Repositories.Showtimes
{
    public class ShowTimeReadRepository : ReadRepository<Showtime>, IShowTimeReadRepository
    {
        public ShowTimeReadRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
