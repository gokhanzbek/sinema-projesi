using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie
{
    public class GetByIdMovieQueryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int DurationInMinutes { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public int ReleaseYear { get; set; }
         // Orijinal PosterUrl yerine senin tercih ettiğin isim!
                                             // Kendi Movie property'lerine göre burayı genişletebilirsin (Description, Duration vs.)
    }
}
