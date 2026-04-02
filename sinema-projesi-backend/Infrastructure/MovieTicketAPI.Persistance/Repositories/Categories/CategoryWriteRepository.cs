using MovieTicketAPI.Application.Repositories.Categories;
using MovieTicketAPI.Domain.Entities;
using MovieTicketAPI.Persistence.Contexts;
using MovieTicketAPI.Persistence.Repositories;

namespace MovieTicketAPI.Persistence.Repositories.Categories
{
    public class CategoryWriteRepository : WriteRepository<Category>, ICategoryWriteRepository
    {
        public CategoryWriteRepository(MovieTicketDbContext context) : base(context)
        {
        }
    }
}
