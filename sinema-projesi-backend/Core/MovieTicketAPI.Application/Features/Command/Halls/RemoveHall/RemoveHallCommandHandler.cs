using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie;
using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Halls.RemoveHall
{
    public class RemoveHallCommandHandler : IRequestHandler<RemoveHallCommandRequest, RemoveHallCommandResponse>
    {
        private readonly IHallReadRepository _hallReadRepository;
        private readonly IHallWriteRepository _hallWriteRepository;

        public RemoveHallCommandHandler(IHallReadRepository hallReadRepository, IHallWriteRepository hallWriteRepository)
        {
            _hallReadRepository = hallReadRepository;
            _hallWriteRepository = hallWriteRepository;
        }

       



        public async Task<RemoveHallCommandResponse> Handle(RemoveHallCommandRequest request, CancellationToken cancellationToken)
        {
           
            

            Hall hall =  await _hallReadRepository.GetByIdAsync(request.Id.ToString(), tracking:true);
            if (hall == null)
            {
                return new RemoveHallCommandResponse { IsSuccess = false, Message = "salon bulunamadı" };
            }
            _hallWriteRepository.Remove(hall);
            await _hallWriteRepository.SaveAsync();
            return new RemoveHallCommandResponse
            {
                IsSuccess = true,
                Message = "Film başarıyla silindi!"
            };
        }
    }
}
