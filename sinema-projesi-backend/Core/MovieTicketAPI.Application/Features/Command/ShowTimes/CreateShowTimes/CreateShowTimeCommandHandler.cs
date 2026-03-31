using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Halls.UpdateHall;
using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.ShowTimes.CreateShowTimes
{
    public class CreateShowTimeCommandHandler : IRequestHandler<CreateShowTimeCommandRequest, CreateShowTimeCommanResponse>
    {
        private readonly IShowTimeWriteRepository _showTimeWriteRepository;

        public CreateShowTimeCommandHandler(IShowTimeWriteRepository showTimeWriteRepository)
        {
            _showTimeWriteRepository = showTimeWriteRepository;
        }

        public async Task<CreateShowTimeCommanResponse> Handle(CreateShowTimeCommandRequest request, CancellationToken cancellationToken)
        {
            

            Showtime newshowtime = new Showtime()
            {

                StartTime = request.StartTime,
                Price = request.Price,
                MovieId = request.MovieId,
                HallId = request.HallId,

            };
            await _showTimeWriteRepository.AddAsync(newshowtime);
            await _showTimeWriteRepository.SaveAsync();
            return new CreateShowTimeCommanResponse()
            {
                Message = "Seans başarıyla oluşturuldu",
                IsSuccess = true,

            };

        }
    }
}
