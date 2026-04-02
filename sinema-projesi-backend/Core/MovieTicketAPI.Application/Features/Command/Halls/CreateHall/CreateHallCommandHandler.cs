using MediatR;
using MovieTicketAPI.Application.Helpers;
using MovieTicketAPI.Application.Repositories.Halls;

namespace MovieTicketAPI.Application.Features.Command.Halls.CreateHall
{
    public class CreateHallCommandHandler : IRequestHandler<CreateHallCommandRequest, CreateHallCommandResponse>
    {
        private readonly IHallWriteRepository _hallWriteRepository;

        public CreateHallCommandHandler(IHallWriteRepository hallWriteRepository)
        {
            _hallWriteRepository = hallWriteRepository;
        }

        public async Task<CreateHallCommandResponse> Handle(CreateHallCommandRequest request, CancellationToken cancellationToken)
        {
            var err = HallGridHelper.ValidateDimensions(request.RowCount, request.ColumnCount);
            if (err != null)
            {
                return new CreateHallCommandResponse
                {
                    IsSuccess = false,
                    Message = err
                };
            }

            var cap = request.RowCount * request.ColumnCount;
            var newHall = new Domain.Entities.Hall
            {
                Name = request.Name,
                RowCount = request.RowCount,
                ColumnCount = request.ColumnCount,
                Capacity = cap
            };
            await _hallWriteRepository.AddAsync(newHall);
            await _hallWriteRepository.SaveAsync();

            return new CreateHallCommandResponse
            {
                IsSuccess = true,
                Message = "Salon başarıyla oluşturuldu.",
                Id = newHall.Id
            };
        }
    }
}
