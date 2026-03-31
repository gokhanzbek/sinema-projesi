using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetShowtimesByMovie
{
    public class GetShowtimesByMovieQueryRequest : IRequest<GetShowtimesByMovieQueryResponse>
    {
        public int MovieId { get; set; }
    }
}
