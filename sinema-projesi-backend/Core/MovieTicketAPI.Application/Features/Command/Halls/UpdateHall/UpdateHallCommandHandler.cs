using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Halls.RemoveHall;
using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

       

        async Task<UpdateHallCommandResponse> IRequestHandler<UpdateHallCommandRequest, UpdateHallCommandResponse>.Handle(UpdateHallCommandRequest request, CancellationToken cancellationToken)
        {
            
            

            Hall newhall = await _hallReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);
            if (newhall == null)
            {
                new UpdateHallCommandResponse() { IsSuccess = false, Message = "Salon bulunamadı" };

            }
            newhall.Capacity = request.Capacity;
            newhall.Name = request.Name;

            _hallWriteRepository.Update(newhall);
            _hallWriteRepository.SaveAsync();
            return new UpdateHallCommandResponse()
            {
                Message = "Salon başarıyla Güncellendi.",
                IsSuccess = true

            };
        }
    }
}
