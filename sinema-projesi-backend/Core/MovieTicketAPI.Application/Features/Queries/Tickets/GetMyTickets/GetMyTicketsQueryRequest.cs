using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Tickets.GetMyTickets
{
    public class GetMyTicketsQueryRequest : IRequest<GetMyTicketsQueryResponse>
    {
    }
}
