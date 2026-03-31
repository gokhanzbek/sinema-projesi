using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories;
using MovieTicketAPI.Application.Repositories.Halls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            

            

            Domain.Entities.Hall NewHall = new Domain.Entities.Hall
            {
                Capacity = request.Capacity,
                Name = request.Name,
            };
            await _hallWriteRepository.AddAsync(NewHall);
            await _hallWriteRepository.SaveAsync();

            return new CreateHallCommandResponse {
                IsSuccess = true,
                Message = "salon başarıyla oluşturuldu"!,
                Id = NewHall.Id
            };


        }
    }
}
