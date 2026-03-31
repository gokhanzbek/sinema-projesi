using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; }           // Ad (nvarchar 150)
        public int DurationInMinutes { get; set; }  // Suresi (int)
        public string Genre { get; set; }           // Tur (nvarchar 50)
        public string ImageUrl { get; set; }
        public string Director { get; set; }        // Yonetmen (nvarchar 100)
        public int ReleaseYear { get; set; }        // Yil (int)
        public string Description { get; set; }     // Aciklama (nvarchar 500)

        
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}
