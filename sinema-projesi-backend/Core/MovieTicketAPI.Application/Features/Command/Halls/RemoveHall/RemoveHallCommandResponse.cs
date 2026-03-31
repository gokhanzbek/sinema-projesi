using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Halls.RemoveHall
{
    public class RemoveHallCommandResponse
    {
        public String Message { get; set; } = null!;
        public bool IsSuccess { get; set; }
    }
}
