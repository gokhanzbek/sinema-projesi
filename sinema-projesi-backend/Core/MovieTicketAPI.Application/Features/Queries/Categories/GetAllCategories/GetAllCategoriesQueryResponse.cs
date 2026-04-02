using System.Collections.Generic;

namespace MovieTicketAPI.Application.Features.Queries.Categories.GetAllCategories
{
    public class GetAllCategoriesQueryResponse
    {
        public List<CategoryItemDto> Categories { get; set; } = new();
    }

    public class CategoryItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
