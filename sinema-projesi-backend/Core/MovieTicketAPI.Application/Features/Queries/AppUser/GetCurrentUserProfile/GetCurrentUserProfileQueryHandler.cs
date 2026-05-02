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
            // 1. ICurrentUser üzerinden sadece ID'yi alıyoruz
            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return new() { Succeeded = false, Message = "Oturum bilgisi alınamadı." };
            }

            // 2. UserManager ile DB'deki en güncel kayda gidiyoruz
            var user = await _userManager.FindByIdAsync(userId);
            

            if (user == null)
            {
                return new() { Succeeded = false, Message = "Kullanıcı veritabanında bulunamadı." };
            }

            // 3. Güncel verileri response olarak dönüyoruz
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
