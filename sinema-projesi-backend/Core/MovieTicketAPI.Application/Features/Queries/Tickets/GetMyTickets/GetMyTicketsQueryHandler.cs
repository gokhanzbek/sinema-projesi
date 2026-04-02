using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Repositories.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Tickets.GetMyTickets
{
    public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQueryRequest, GetMyTicketsQueryResponse>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ICurrentUser _currentUser;

        public GetMyTicketsQueryHandler(ITicketReadRepository ticketReadRepository, ICurrentUser currentUser)
        {
            _ticketReadRepository = ticketReadRepository;
            _currentUser = currentUser;
        }

        public async Task<GetMyTicketsQueryResponse> Handle(GetMyTicketsQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. Token'dan adamın ID'sini cımbızla çekiyoruz!
            if (!int.TryParse(_currentUser.UserId, out int userId))
            {
                return new GetMyTicketsQueryResponse { Succeeded = false, Message = "Lütfen önce giriş yapın!" };
            }

            // 2. Veritabanından SADECE bu adama ait biletleri getiriyoruz
            var myTickets = await _ticketReadRepository
                .GetWhere(t => t.AppUserId == userId)
                .Select(t => new
                {
                    t.Id,
                    t.SeatNumber,
                    t.Price,
                    t.ShowtimeId,
                    MovieTitle = t.Showtime.Movie.Title,
                    ShowtimeStart = t.Showtime.StartTime,
                    Status = t.Status
                })
                .ToListAsync(cancellationToken);

            return new GetMyTicketsQueryResponse
            {
                Succeeded = true,
                Message = $"{myTickets.Count} adet biletiniz bulundu.",
                Tickets = myTickets
            };
        }
    }
}

