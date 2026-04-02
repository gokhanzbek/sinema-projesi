using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Abstractions.Services
{
    public interface ICurrentUser
    {
        string? UserId { get; }
    }
}
