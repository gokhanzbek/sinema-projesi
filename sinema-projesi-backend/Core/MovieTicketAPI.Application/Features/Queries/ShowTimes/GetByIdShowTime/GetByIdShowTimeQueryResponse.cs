using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetByIdShowTime
{
    public class GetByIdShowTimeQueryResponse
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }

        // İleride filmin adı, salonun adı gibi ekstra bilgileri buraya ekleyebilirsin.
    }
}
