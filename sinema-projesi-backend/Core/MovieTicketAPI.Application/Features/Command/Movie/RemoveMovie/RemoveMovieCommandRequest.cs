using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie
{
    public class RemoveMovieCommandRequest : IRequest<RemoveMovieCommandResponse>
    {
        public int Id { get; set; }
    }
}
