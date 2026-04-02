using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.CreateMovie
{
    public class CreateMovieCommandResponse 
    {
        public int Id { get; set; }
        public String Message { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public decimal? ImdbRating { get; set; }
    }
}
