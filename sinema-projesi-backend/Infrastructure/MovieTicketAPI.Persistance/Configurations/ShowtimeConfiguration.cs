using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence.Configurations
{
    public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
    {
        public void Configure(EntityTypeBuilder<Showtime> builder)
        {
            builder.HasKey(s => s.Id);

            // Senin belirlediğin datetime2 ve decimal(8,2) kuralları!
            builder.Property(s => s.StartTime).IsRequired().HasColumnType("datetime2");
            builder.Property(s => s.Price).IsRequired().HasColumnType("decimal(8,2)");

            // İlişkiler (Foreign Key atamaları)
            builder.HasOne(s => s.Movie)
                   .WithMany(m => m.Showtimes)
                   .HasForeignKey(s => s.MovieId);

            builder.HasOne(s => s.Hall)
                   .WithMany(h => h.Showtimes)
                   .HasForeignKey(s => s.HallId);
        }
    }
}
