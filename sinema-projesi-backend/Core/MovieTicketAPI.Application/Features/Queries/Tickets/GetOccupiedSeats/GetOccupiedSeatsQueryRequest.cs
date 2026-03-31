using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Tickets.GetOccupiedSeats
{
    public class GetOccupiedSeatsQueryRequest : IRequest<GetOccupiedSeatsQueryResponse>
    {
        public int ShowtimeId { get; set; }
    }
}
