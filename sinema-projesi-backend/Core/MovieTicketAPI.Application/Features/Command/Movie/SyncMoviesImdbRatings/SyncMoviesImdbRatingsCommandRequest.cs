using MediatR;

namespace MovieTicketAPI.Application.Features.Command.Movie.SyncMoviesImdbRatings
{
    public class SyncMoviesImdbRatingsCommandRequest : IRequest<SyncMoviesImdbRatingsCommandResponse>
    {
    }
}
