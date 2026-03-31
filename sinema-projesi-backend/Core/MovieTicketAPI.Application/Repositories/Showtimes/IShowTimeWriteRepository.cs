using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Repositories.Showtimes
{
    public interface IShowTimeWriteRepository : IWriteRepository<Showtime>
    {
    }
}
