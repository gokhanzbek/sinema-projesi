using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities
{
    public class Hall : BaseEntity
    {
        public string Name { get; set; }     // SalonAdi (nvarchar 50)
        public int Capacity { get; set; }    // Kapasite (int)

        // İlişki: Bir salonun birden fazla seansı olabilir
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}

