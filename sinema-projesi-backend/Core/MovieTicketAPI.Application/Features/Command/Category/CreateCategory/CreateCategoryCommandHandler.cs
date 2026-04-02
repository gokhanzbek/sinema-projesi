using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Categories;
using System.Threading;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Category.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ICategoryWriteRepository _categoryWriteRepository;

        public CreateCategoryCommandHandler(
            ICategoryReadRepository categoryReadRepository,
            ICategoryWriteRepository categoryWriteRepository)
        {
            _categoryReadRepository = categoryReadRepository;
            _categoryWriteRepository = categoryWriteRepository;
        }

        public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            var trimmed = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return new CreateCategoryCommandResponse { IsSuccess = false, Message = "Kategori adı boş olamaz." };
            }

            var exists = await _categoryReadRepository.GetWhere(c => c.Name == trimmed, false)
                .AnyAsync(cancellationToken);
            if (exists)
            {
                return new CreateCategoryCommandResponse { IsSuccess = false, Message = "Bu isimde bir kategori zaten var." };
            }

            var entity = new MovieTicketAPI.Domain.Entities.Category { Name = trimmed };
            await _categoryWriteRepository.AddAsync(entity);
            await _categoryWriteRepository.SaveAsync();

            return new CreateCategoryCommandResponse
            {
                IsSuccess = true,
                Message = "Kategori eklendi.",
                Id = entity.Id
            };
        }
    }
}
