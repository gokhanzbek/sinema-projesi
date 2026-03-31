using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Repositories.Halls
{
    public class HallWriteRepository : WriteRepository<Hall>, IHallWriteRepository
    {
        public HallWriteRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
