using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.ShowTimes.CreateShowTimes
{
    public class CreateShowTimeCommandRequest :IRequest<CreateShowTimeCommanResponse>
    {
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
    }
}
