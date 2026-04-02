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
        /// <summary>Toplam koltuk = RowCount * ColumnCount (oluşturma/güncellemede senkron tutulur).</summary>
        public int Capacity { get; set; }
        /// <summary>Sıra sayısı (A, B, C… tek harf; en fazla 26).</summary>
        public int RowCount { get; set; }
        /// <summary>Her sıradaki koltuk sayısı.</summary>
        public int ColumnCount { get; set; }

        // İlişki: Bir salonun birden fazla seansı olabilir
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}

