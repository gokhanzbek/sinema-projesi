using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Tickets.RemoveTicket
{
    public class RemoveTicketCommandHandler : IRequestHandler<RemoveTicketCommandRequest, RemoveTicketCommandResponse>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ICurrentUser _currentUser;

        public RemoveTicketCommandHandler(
            ITicketReadRepository ticketReadRepository,
            ITicketWriteRepository ticketWriteRepository,
            ICurrentUser currentUser)
        {
            _ticketReadRepository = ticketReadRepository;
            _ticketWriteRepository = ticketWriteRepository;
            _currentUser = currentUser;
        }

        public async Task<RemoveTicketCommandResponse> Handle(RemoveTicketCommandRequest request, CancellationToken cancellationToken)
        {
            // 1. İşlemi yapan adamın ID'sini al
            if (!int.TryParse(_currentUser.UserId, out int userId))
            {
                return new RemoveTicketCommandResponse { Succeeded = false, Message = "Lütfen önce giriş yapın!" };
            }

            // 2. İptal edilmek istenen bileti veritabanından bul
            // (GetByIdAsync metodun string bekliyorsa request.TicketId.ToString() yapabilirsin)
            var ticket = await _ticketReadRepository.GetByIdAsync(request.TicketId.ToString());

            if (ticket == null)
            {
                return new RemoveTicketCommandResponse { Succeeded = false, Message = "Böyle bir bilet bulunamadı!" };
            }

            // 3. ZIRH: Bu bilet bu adama mı ait? (Başkası hacklemeye çalışıyor olabilir)
            if (ticket.AppUserId != userId)
            {
                return new RemoveTicketCommandResponse { Succeeded = false, Message = "Sadece kendi biletlerinizi iptal edebilirsiniz!" };
            }

            // 4. Her şey tamamsa bileti sil (Koltuk artık boşa çıktı!)
            await _ticketWriteRepository.RemoveAsync(ticket.Id.ToString());
            await _ticketWriteRepository.SaveAsync();

            return new RemoveTicketCommandResponse
            {
                Succeeded = true,
                Message = "Biletiniz başarıyla iptal edildi ve koltuğunuz boşa çıktı."
            };
        }
    }
}
