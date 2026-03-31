using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MovieTicketAPI.Application.Exceptions;

namespace MovieTicketAPI.Application.Features.Command.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        //USERMANAGER service'i sayesinde repostitory eklememize gerek kalmıyor.(service registration'da tanımladık.
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {

            // IdentityUser'dan türettiğimiz AppUser nesnesini oluşturuyoruz
            IdentityResult result = await _userManager.CreateAsync(new()
            {

                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName, // Yeni yapıya göre eklendi
                LastName = request.LastName
            }, request.Password);


            if (result.Succeeded)
            {
                return new()
                {
                    Succeeded = true,
                    Message = "kullanıcı başarıyla oluşturulmuştur."
                };
            }
            // Başarısız durum: Identity hatalarını da içerecek şekilde dönmek daha iyidir
            return new CreateUserCommandResponse
            {
                Succeeded = false,
                Message = string.Join(" | ",
                    result.Errors.Select(e => e.Description))
            };

        }

    }
}


