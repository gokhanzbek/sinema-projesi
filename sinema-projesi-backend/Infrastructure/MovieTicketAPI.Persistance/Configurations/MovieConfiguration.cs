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
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Genre).HasMaxLength(50);
            builder.Property(m => m.Director).HasMaxLength(100);
            builder.Property(m => m.Description).HasMaxLength(500);
            builder.Property(m => m.DurationInMinutes).IsRequired();
            builder.Property(m => m.ImageUrl).IsRequired(false).HasMaxLength(500);
        }
    }
}
