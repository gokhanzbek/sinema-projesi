using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetAllMovies
{
    public class GetAllMoviesQueryResponse
    {
        public int TotalCount { get; set; }
        public object Movies { get; set; }
    }
}
