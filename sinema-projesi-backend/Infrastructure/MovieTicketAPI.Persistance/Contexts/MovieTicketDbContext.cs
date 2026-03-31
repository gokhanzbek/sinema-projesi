using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Domain.Entities.Common;
using MovieTicketAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Contexts
{
    public class MovieTicketDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public MovieTicketDbContext(DbContextOptions<MovieTicketDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        // IdentityDbSet'ler (Users, Roles vb.) arka planda otomatik olarak geliyor.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity tablolarının oluşması için base.OnModelCreating(modelBuilder) ÇAĞRILMAK ZORUNDADIR!
            base.OnModelCreating(modelBuilder);

            // Aynı seans (ShowtimeId) ve aynı koltuk (SeatNumber) bir daha satılamaz!
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.ShowtimeId, t.SeatNumber })
                .IsUnique();

            // Yazdığımız Configuration sınıflarını (MovieConfiguration vb.) otomatik bulup uygular
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
