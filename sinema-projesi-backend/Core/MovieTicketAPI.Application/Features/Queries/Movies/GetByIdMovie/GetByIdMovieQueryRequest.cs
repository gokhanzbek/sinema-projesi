using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie
{
    public class GetByIdMovieQueryRequest : IRequest<GetByIdMovieQueryResponse> { public int Id { get; set; } }
}
