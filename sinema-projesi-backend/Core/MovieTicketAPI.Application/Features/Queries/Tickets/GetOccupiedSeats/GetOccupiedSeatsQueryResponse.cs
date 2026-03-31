using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Tickets.GetOccupiedSeats
{
    public class GetOccupiedSeatsQueryResponse
    {
        // Sadece dolu koltukların isimlerini dönüyoruz ("G12", "G13" vs.)
        public List<string> OccupiedSeats { get; set; }
    }
}
