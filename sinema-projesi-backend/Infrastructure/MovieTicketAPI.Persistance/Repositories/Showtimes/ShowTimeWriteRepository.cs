using MovieTicketAPI.Application.Repositories;
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
    public class ShowTimeWriteRepository : WriteRepository<Showtime>, IShowTimeWriteRepository
    {
        public ShowTimeWriteRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
