using MovieTicketAPI.Application.Repositories.Tickets;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Repositories.Tickets
{
    public class TicketReadRepository : ReadRepository<Ticket>, ITicketReadRepository
    {
        public TicketReadRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
