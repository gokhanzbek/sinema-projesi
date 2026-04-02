using MovieTicketAPI.Application.Repositories;
using MovieTicketAPI.Domain.Entities;

namespace MovieTicketAPI.Application.Repositories.Categories
{
    public interface ICategoryWriteRepository : IWriteRepository<Category>
    {
    }
}
