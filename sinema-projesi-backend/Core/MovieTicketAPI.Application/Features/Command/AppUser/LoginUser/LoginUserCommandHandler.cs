using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketAPI.Application.Abstractions.Token;
using MovieTicketAPI.Application.DTOs;
using MovieTicketAPI.Application.Exceptions;

namespace MovieTicketAPI.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;

        public LoginUserCommandHandler(
            UserManager<Domain.Entities.Identity.AppUser> userManager,
            SignInManager<Domain.Entities.Identity.AppUser> signInManager,
            ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            //kullanıcı adı girilirse tetiklenir
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);//email girişi yapılırsa tetiklenir
            }

            if (user == null)
            {
                throw new NotFoundUserException();
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            
            if (result.Succeeded) // Authentication başarılı!
            {
                // 1. İŞTE YENİ EKLENEN KISIM: Adamın rollerini veritabanından çekiyoruz
                IList<string> userRoles = await _userManager.GetRolesAsync(user);

                // 2. Çektiğimiz rolleri TokenHandler'a gönderiyoruz ki içine kazısın!
                Token token = _tokenHandler.CreateAccessToken(900, user, userRoles); 
                
                return new LoginUserSuccessCommandResponse()
                {
                    Token = token
                };
            }
            
            throw new AuthenticationErrorException();
        }
    }
}