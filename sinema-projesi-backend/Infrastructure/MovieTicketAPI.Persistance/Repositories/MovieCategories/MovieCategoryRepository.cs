using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.MovieCategories;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;

namespace MovieTicketAPI.Persistence.Repositories.MovieCategories
{
    public class MovieCategoryRepository : IMovieCategoryRepository
    {
        private readonly MovieTicketDbContext _context;

        public MovieCategoryRepository(MovieTicketDbContext context)
        {
            _context = context;
        }

        public async Task SetCategoriesForMovieAsync(int movieId, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default)
        {
            var existing = await _context.MovieCategories.Where(mc => mc.MovieId == movieId).ToListAsync(cancellationToken);
            _context.MovieCategories.RemoveRange(existing);
            foreach (var cid in categoryIds.Distinct())
            {
                _context.MovieCategories.Add(new MovieCategory { MovieId = movieId, CategoryId = cid });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
