using MediatR;

namespace MovieTicketAPI.Application.Features.Queries.AppUser.GetCurrentUserProfile
{
    public class GetCurrentUserProfileQueryRequest : IRequest<GetCurrentUserProfileQueryResponse>
    {
    }
}
