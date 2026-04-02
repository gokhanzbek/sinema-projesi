using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Repositories.MovieCategories
{
    public interface IMovieCategoryRepository
    {
        Task SetCategoriesForMovieAsync(int movieId, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default);
    }
}
