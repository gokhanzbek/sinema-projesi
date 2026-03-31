using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Tickets.RemoveTicket
{
    public class RemoveTicketCommandRequest : IRequest<RemoveTicketCommandResponse>
    {
        public int TicketId { get; set; } // Hangi bilet iptal edilecek?
    }
}
