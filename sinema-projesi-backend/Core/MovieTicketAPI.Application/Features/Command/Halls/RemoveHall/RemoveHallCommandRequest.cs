using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Halls.RemoveHall
{
    public class RemoveHallCommandRequest : IRequest<RemoveHallCommandResponse>
    {
        public int Id { get; set; }
    }
}
