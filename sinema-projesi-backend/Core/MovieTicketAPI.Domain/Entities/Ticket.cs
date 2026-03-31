using MovieTicketAPI.Domain.Entities.Common;
using MovieTicketAPI.Domain.Entities.Identity;
using MovieTicketAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities
{
    // Bilet Durumları için Enum
   
    public class Ticket : BaseEntity
    {
        // 1. Müşteri (AppUser) Bağlantısı
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        // 2. Seans Bağlantısı (Showtime ID'nin tipi projende neyse ona göre int veya Guid yapmalısın)
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }

        // 3. Bilet Detayları
        public string SeatNumber { get; set; } = null!;
        public decimal Price { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Active;
    }
}
