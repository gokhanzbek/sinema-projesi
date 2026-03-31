using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.ShowTimes.UpdateShowTimes
{
    public class UpdateShowTimeCommandRequest : IRequest<UpdateShowTimeCommandResponse>
    {
        public int Id { get; set; } // Hangi seans güncellenecek?

        // Yeni değerler:
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
    }
}
