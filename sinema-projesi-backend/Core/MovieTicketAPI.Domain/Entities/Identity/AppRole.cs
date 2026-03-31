using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities.Identity
{
    public class AppRole : IdentityRole<int>
    {
        // Temel IdentityRole bize Name, NormalizedName gibi alanları zaten veriyor.
        // İstersek buraya ekstra rol özellikleri ekleyebiliriz.
    }
}
