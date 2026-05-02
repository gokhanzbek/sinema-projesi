using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Abstractions.Token;
using MovieTicketAPI.Infrastructure.Services.Omdb;
using MovieTicketAPI.Infrastructure.Services.Token;

namespace MovieTicketAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<OmdbOptions>(configuration.GetSection(OmdbOptions.SectionName));
            serviceCollection.AddHttpClient<IOmdbMovieRatingService, OmdbMovieRatingService>();

            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
        }
    }
}
