using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie
{
    public class RemoveMovieCommandResponse
    {
        public String Message { get; set; } = null!;
        public bool IsSuccess { get; set; }
    }
}
