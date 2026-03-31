using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.CreateMovie
{
    public class CreateMovieCommandRequest : IRequest<CreateMovieCommandResponse>
    {
        public string Title { get; set; }           
        public int DurationInMinutes { get; set; } 
        public string Genre { get; set; }           
        public string Director { get; set; }       
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
