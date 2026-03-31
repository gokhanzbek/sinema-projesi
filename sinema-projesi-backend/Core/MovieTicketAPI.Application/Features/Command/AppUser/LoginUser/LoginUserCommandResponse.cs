using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketAPI.Application.DTOs;

namespace MovieTicketAPI.Application.Features.Command.AppUser.LoginUser
{
    public class LoginUserCommandResponse
    {
       
        
    }
    public class LoginUserSuccessCommandResponse : LoginUserCommandResponse
    {
        public Token Token { get; set; }
    }
    public class LoginUserErrorCommandResponse : LoginUserCommandResponse
    {
        public string Message { get; set; }
    }

}
