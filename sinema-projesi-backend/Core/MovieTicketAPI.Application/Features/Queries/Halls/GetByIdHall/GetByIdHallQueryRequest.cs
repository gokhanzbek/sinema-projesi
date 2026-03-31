using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Halls.GetByIdHall
{
    public class GetByIdHallQueryRequest : IRequest<GetByIdHallQueryResponse> { public int Id { get; set; } }
}
