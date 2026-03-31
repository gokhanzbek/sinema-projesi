using MovieTicketAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        DTOs.Token CreateAccessToken(int minute, AppUser user, IList<string> roles);
        //isim çakışması oldu o yüzden DTOs dan geldiğini belirttik
    }
}
