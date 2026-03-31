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

namespace MovieTicketAPI.Application.Features.Command.ShowTimes.UpdateShowTimes
{
    public class UpdateShowTimeCommandHandler : IRequestHandler<UpdateShowTimeCommandRequest, UpdateShowTimeCommandResponse>
    {
        private readonly IShowTimeReadRepository _showTimeReadRepository;
        private readonly IShowTimeWriteRepository _showTimeWriteRepository;

        public UpdateShowTimeCommandHandler(IShowTimeReadRepository showTimeReadRepository, IShowTimeWriteRepository showTimeWriteRepository)
        {
            _showTimeReadRepository = showTimeReadRepository;
            _showTimeWriteRepository = showTimeWriteRepository;
        }




        // Dependency Injection


        public async Task<UpdateShowTimeCommandResponse> Handle(UpdateShowTimeCommandRequest request, CancellationToken cancellationToken)
        {
           
            // 1. Güncellenecek seansı veritabanından bul (Yine tracking: true yapıyoruz ki EF Core bu nesneyi izlesin)
            Showtime showTime = await _showTimeReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);

            // 2. Eğer böyle bir seans bulunduysa, içindeki eski değerleri dışarıdan gelen (request) yeni değerlerle ez
            if (showTime != null)
            {
                showTime.StartTime = request.StartTime;
                showTime.Price = request.Price;
                showTime.MovieId = request.MovieId;
                showTime.HallId = request.HallId;

                // 3. Güncelleme işlemini Write Repository'e bildir ve veritabanına kaydet
                _showTimeWriteRepository.Update(showTime);
                await _showTimeWriteRepository.SaveAsync();
            }

            // 4. Başarı cevabını dön
            return new UpdateShowTimeCommandResponse();
        }
    }
}
