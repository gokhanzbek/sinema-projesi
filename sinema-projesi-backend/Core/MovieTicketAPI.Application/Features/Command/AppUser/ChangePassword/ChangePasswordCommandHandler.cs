using MediatR;
using Microsoft.AspNetCore.Identity;
using MovieTicketAPI.Application.Abstractions.Services;

namespace MovieTicketAPI.Application.Features.Command.AppUser.ChangePassword
{
    public class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommandRequest, ChangePasswordCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        private readonly ICurrentUser _currentUser;

        public ChangePasswordCommandHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<ChangePasswordCommandResponse> Handle(
            ChangePasswordCommandRequest request,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(_currentUser.UserId, out var userId))
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = false,
                    Message = "Oturum bulunamadı. Lütfen tekrar giriş yapın."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = false,
                    Message = "Yeni şifre boş olamaz."
                };
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = false,
                    Message = "Yeni şifre ile tekrarı eşleşmiyor."
                };
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (result.Succeeded)
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = true,
                    Message = "Şifreniz güncellendi."
                };
            }

            return new ChangePasswordCommandResponse
            {
                Succeeded = false,
                Message = string.Join(" ", result.Errors.Select(e => e.Description))
            };
        }
    }
}
