using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Tickets.CreateTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommandRequest, CreateTicketCommandResponse>
    {
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ITicketReadRepository _ticketReadRepository;

        public CreateTicketCommandHandler(ITicketWriteRepository ticketWriteRepository, ICurrentUser currentUser, ITicketReadRepository ticketReadRepository)
        {
            _ticketWriteRepository = ticketWriteRepository;
            _currentUser = currentUser;
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<CreateTicketCommandResponse> Handle(CreateTicketCommandRequest request, CancellationToken cancellationToken)
        {
            
            if (!int.TryParse(_currentUser.UserId, out int userId))
            {
                return new CreateTicketCommandResponse
                {
                    Succeeded = false,
                    Message = "Yetkisiz işlem veya geçersiz kullanıcı kimliği."
                };
            }
            // 2. İŞTE YENİ ZIRHIMIZ: İstenen koltuklar zaten dolu mu? (ASENKRON VERSİYON)
            var takenSeats = await _ticketReadRepository
                .GetWhere(t => t.ShowtimeId == request.ShowtimeId && request.SeatNumbers.Contains(t.SeatNumber))
                .Select(t => t.SeatNumber)
                .ToListAsync(cancellationToken); // ToList() gitti, yerine aslanlar gibi await ve ToListAsync geldi!

            // Eğer içeride dolu koltuk bulursak, sistemi patlatmadan kibarca uyarıp işlemi iptal ediyoruz!
            if (takenSeats.Any())
            {
                string doluKoltuklar = string.Join(", ", takenSeats);
                return new CreateTicketCommandResponse
                {
                    Succeeded = false,
                    Message = $"İşlem başarısız! Seçtiğiniz şu koltuklar zaten dolu: {doluKoltuklar}"
                };
            }


            // 2. Biletleri tek tek yollamak yerine bir listede topluyoruz
            var newTickets = new List<Domain.Entities.Ticket>();

            foreach (var seat in request.SeatNumbers)
            {
                newTickets.Add(new Domain.Entities.Ticket
                {
                    AppUserId = userId,
                    ShowtimeId = request.ShowtimeId,
                    SeatNumber = seat,
                    Price = request.Price, // Not: İleride fiyatı request'ten almak yerine veritabanındaki (Showtime) fiyattan çekmek daha güvenlidir! (Adam request'i hackleyip 1 TL yollayabilir)
                    Status = Domain.Enums.TicketStatus.Active
                });
            }

            // 3. Tek seferde (Bulk) ekleme yapıyoruz!
            await _ticketWriteRepository.AddRangeAsync(newTickets);

            // İleride buraya try-catch ekleyip Unique Constraint patlamasını yakalayabilirsin
            await _ticketWriteRepository.SaveAsync();

            return new CreateTicketCommandResponse
            {
                Succeeded = true,
                Message = $"{request.SeatNumbers.Count} adet bilet başarıyla satın alındı!"
            };
        }
    }
}

