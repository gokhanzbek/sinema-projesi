using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Tickets.CreateTicket
{
    public class CreateTicketCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
