using MovieTicketAPI.Application.Repositories.Categories;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using MovieTicketAPI.Persistence.Repositories;

namespace MovieTicketAPI.Persistence.Repositories.Categories
{
    public class CategoryReadRepository : ReadRepository<Category>, ICategoryReadRepository
    {
        public CategoryReadRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
