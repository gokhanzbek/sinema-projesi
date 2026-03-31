using Microsoft.Extensions.DependencyInjection;
using MovieTicketAPI.Application.Abstractions.Token;
using MovieTicketAPI.Infrastructure.Services.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {


            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();


        }
    }
}
