using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Categories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Categories.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQueryRequest, GetAllCategoriesQueryResponse>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;

        public GetAllCategoriesQueryHandler(ICategoryReadRepository categoryReadRepository)
        {
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<GetAllCategoriesQueryResponse> Handle(GetAllCategoriesQueryRequest request, CancellationToken cancellationToken)
        {
            var items = await _categoryReadRepository.GetAll(false)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryItemDto { Id = c.Id, Name = c.Name })
                .ToListAsync(cancellationToken);

            return new GetAllCategoriesQueryResponse { Categories = items };
        }
    }
}
