using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Halls.GetAllHalls
{
    public class GetAllHallsQueryRequest : IRequest<GetAllHallsQueryResponse> { }
}
