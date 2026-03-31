using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Halls.CreateHall
{
    public class CreateHallCommandResponse
    {
        public int Id { get; set; }
        public String Message { get; set; } = null!;
        public bool IsSuccess { get; set; }
    }
}
