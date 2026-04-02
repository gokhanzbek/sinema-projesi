using MediatR;
using Microsoft.AspNetCore.Identity;
using MovieTicketAPI.Application.Abstractions.Services;

namespace MovieTicketAPI.Application.Features.Queries.AppUser.GetCurrentUserProfile
{
    public class GetCurrentUserProfileQueryHandler
        : IRequestHandler<GetCurrentUserProfileQueryRequest, GetCurrentUserProfileQueryResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly ICurrentUser _currentUser;

        public GetCurrentUserProfileQueryHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<GetCurrentUserProfileQueryResponse> Handle(
            GetCurrentUserProfileQueryRequest request,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(_currentUser.UserId, out var userId))
            {
                return new GetCurrentUserProfileQueryResponse
                {
                    Succeeded = false,
                    Message = "Oturum bulunamadı. Lütfen tekrar giriş yapın."
                };
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new GetCurrentUserProfileQueryResponse
                {
                    Succeeded = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            return new GetCurrentUserProfileQueryResponse
            {
                Succeeded = true,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
