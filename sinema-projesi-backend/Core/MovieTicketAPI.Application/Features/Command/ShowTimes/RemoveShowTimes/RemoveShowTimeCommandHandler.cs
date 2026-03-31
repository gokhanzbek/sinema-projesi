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

namespace MovieTicketAPI.Application.Features.Command.ShowTimes.RemoveShowTimes
{
    public class RemoveShowTimeCommandHandler : IRequestHandler<RemoveShowTimeCommandRequest, RemoveShowTimeCommandResponse>
    {
        private readonly IShowTimeReadRepository _showTimeReadRepository;
        private readonly IShowTimeWriteRepository _showTimeWriteRepository;

        public RemoveShowTimeCommandHandler(IShowTimeReadRepository showTimeReadRepository, IShowTimeWriteRepository showTimeWriteRepository)
        {
            _showTimeReadRepository = showTimeReadRepository;
            _showTimeWriteRepository = showTimeWriteRepository;
        }

        public async Task<RemoveShowTimeCommandResponse> Handle(RemoveShowTimeCommandRequest request, CancellationToken cancellationToken)
        {
            

            Showtime showTime = await _showTimeReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);

            // 2. Eğer varsa sil
            if (showTime != null)
            {
                _showTimeWriteRepository.Remove(showTime);
                await _showTimeWriteRepository.SaveAsync();
            }

            // 3. Cevabı dön
            return new RemoveShowTimeCommandResponse();
        }
    }
}
