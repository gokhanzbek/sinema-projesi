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
        public List<string> Categories { get; set; }
        public List<int> CategoryIds { get; set; }
        public string Director { get; set; }
        public int ReleaseYear { get; set; }
        public string? ImdbId { get; set; }
        public decimal? ImdbRating { get; set; }
    }
}
