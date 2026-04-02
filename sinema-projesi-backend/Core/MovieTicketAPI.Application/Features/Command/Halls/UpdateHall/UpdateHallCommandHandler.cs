using MediatR;
using MovieTicketAPI.Application.Helpers;
using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Domain.Entities;

namespace MovieTicketAPI.Application.Features.Command.Halls.UpdateHall
{
    public class UpdateHallCommandHandler : IRequestHandler<UpdateHallCommandRequest, UpdateHallCommandResponse>
    {
        private readonly IHallReadRepository _hallReadRepository;
        private readonly IHallWriteRepository _hallWriteRepository;

        public UpdateHallCommandHandler(IHallReadRepository hallReadRepository, IHallWriteRepository hallWriteRepository)
        {
            _hallReadRepository = hallReadRepository;
            _hallWriteRepository = hallWriteRepository;
        }

        public async Task<UpdateHallCommandResponse> Handle(UpdateHallCommandRequest request, CancellationToken cancellationToken)
        {
            var dimErr = HallGridHelper.ValidateDimensions(request.RowCount, request.ColumnCount);
            if (dimErr != null)
            {
                return new UpdateHallCommandResponse { IsSuccess = false, Message = dimErr };
            }

            var hall = await _hallReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);
            if (hall == null)
            {
                return new UpdateHallCommandResponse { IsSuccess = false, Message = "Salon bulunamadı." };
            }

            hall.Name = request.Name;
            hall.RowCount = request.RowCount;
            hall.ColumnCount = request.ColumnCount;
            hall.Capacity = request.RowCount * request.ColumnCount;

            _hallWriteRepository.Update(hall);
            await _hallWriteRepository.SaveAsync();
            return new UpdateHallCommandResponse
            {
                Message = "Salon başarıyla güncellendi.",
                IsSuccess = true
            };
        }
    }
}
