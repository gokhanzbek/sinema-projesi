using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Enums
{
    public enum TicketStatus
    {
        Active = 1,   // Aktif / Geçerli
        Canceled = 2, // İptal Edildi
        Past = 3      // Geçmiş (İstersen bunu enum'da tutabilir veya anlık hesaplayabiliriz)
    }
}
