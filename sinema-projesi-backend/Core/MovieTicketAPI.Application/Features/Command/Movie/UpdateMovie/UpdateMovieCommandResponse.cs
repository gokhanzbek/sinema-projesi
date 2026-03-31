using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie
{
    public class UpdateMovieCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
    }
}
