using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Tickets.CreateTicket
{
    public class CreateTicketCommandRequest : IRequest<CreateTicketCommandResponse>
    {
        public int ShowtimeId { get; set; }

        // Tek seferde birden fazla koltuk alınabilmesi için liste yapıyoruz! (Örn: ["A-1", "A-2"])
        public List<string> SeatNumbers { get; set; } = new List<string>();

        public decimal Price { get; set; } // Bilet başı fiyat
    }
}
