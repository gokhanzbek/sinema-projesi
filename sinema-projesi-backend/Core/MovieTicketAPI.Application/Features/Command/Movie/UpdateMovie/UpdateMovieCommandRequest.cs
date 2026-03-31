using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie
{
    public class UpdateMovieCommandRequest : IRequest<UpdateMovieCommandResponse>
    {
        public int Id { get; set; }  // Hangi görev güncellenecek?
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCompleted { get; set; } // Görev tamamlandı mı?
    }
}
