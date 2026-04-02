using MediatR;

namespace MovieTicketAPI.Application.Features.Queries.Categories.GetAllCategories
{
    public class GetAllCategoriesQueryRequest : IRequest<GetAllCategoriesQueryResponse> { }
}
