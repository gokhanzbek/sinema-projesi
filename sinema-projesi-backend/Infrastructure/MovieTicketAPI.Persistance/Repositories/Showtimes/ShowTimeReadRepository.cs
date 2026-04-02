using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;

namespace MovieTicketAPI.Persistence.Repositories.Showtimes
{
    public class ShowTimeReadRepository : ReadRepository<Showtime>, IShowTimeReadRepository
    {
        public ShowTimeReadRepository(MovieTicketDbContext context) : base(context)
        {
        }

        public Task<Showtime?> GetByIdWithHallAsync(int id, CancellationToken cancellationToken = default) =>
            Table.AsNoTracking()
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
