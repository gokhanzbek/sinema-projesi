using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities
{
    public class Showtime : BaseEntity
    {
        public DateTime StartTime { get; set; } // BaslangicZamani (datetime2)
        public decimal Price { get; set; }      // Fiyat (decimal 8,2)

        // İlişki: Hangi Film? (Foreign Key -> FilmId)
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // İlişki: Hangi Salon? (Foreign Key -> SalonId)
        public int HallId { get; set; }
        public Hall Hall { get; set; }
    }
}
