using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
 using MovieTicketAPI.Application.Abstractions.Services; 
using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
 using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories; // Bu satır hatalı görünüyordu, düzelttim.
using MovieTicketAPI.Application.Repositories.Categories;
using MovieTicketAPI.Application.Repositories.MovieCategories;
using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Application.Repositories.Tickets;
using MovieTicketAPI.Domain.Entities.Identity;
using MovieTicketAPI.Persistence.Contexts;
using MovieTicketAPI.Persistence.Repositories.Categories;
using MovieTicketAPI.Persistence.Repositories.Halls;
using MovieTicketAPI.Persistence.Repositories.MovieCategories;
using MovieTicketAPI.Persistence.Repositories.Movies;
using MovieTicketAPI.Persistence.Repositories.Showtimes;
using MovieTicketAPI.Persistence.Repositories.Tickets;
 using MovieTicketAPI.Persistence.Services;

namespace MovieTicketAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. DbContext Kaydı
            services.AddDbContext<MovieTicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Identity Kaydı
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })


            .AddEntityFrameworkStores<MovieTicketDbContext>()
            .AddDefaultTokenProviders(); // 1. EKLEME: Varsayılan token sağlayıcıları eklendi

            // 3. Current User Servisi (Senin özel eklediğin)
            // 2. EKLEME: CurrentUser servisini projeye tanıttık. (Hata verirse yukarıdaki using satırlarını açmalısın)
            // services.AddScoped<ICurrentUser, CurrentUser>(); 

            // --- MOVIE ---
            services.AddScoped<IMovieReadRepository, MovieReadRepository>();
            services.AddScoped<IMovieWriteRepository, MovieWriteRepository>();

            // --- CATEGORY ---
            services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
            services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();
            services.AddScoped<IMovieCategoryRepository, MovieCategoryRepository>();

            // --- HALL ---
            services.AddScoped<IHallReadRepository, HallReadRepository>();
            services.AddScoped<IHallWriteRepository, HallWriteRepository>();

            // --- SHOWTIME ---
            services.AddScoped<IShowTimeReadRepository, ShowTimeReadRepository>();
            services.AddScoped<IShowTimeWriteRepository, ShowTimeWriteRepository>();

            //---Ticket---
            services.AddScoped<ITicketReadRepository, TicketReadRepository>();
            services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();

            services.AddScoped<ICurrentUser, CurrentUser>();
        }
    }
}
