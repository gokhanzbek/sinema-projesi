using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieTicketAPI.Application.Abstractions.Token;
using MovieTicketAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTicketAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // DİKKAT: Buraya IList<string> roles parametresini ekledik!
        public Application.DTOs.Token CreateAccessToken(int minute, AppUser user, IList<string> roles)
        {
            Application.DTOs.Token token = new();
            
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.UtcNow.AddMinutes(minute);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.UserName) 
            };

            // İŞTE SİHİRLİ DOKUNUŞ: Adamın rollerini Token'a basıyoruz!
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            JwtSecurityToken securityToken = new JwtSecurityToken(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                claims: claims, 
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}