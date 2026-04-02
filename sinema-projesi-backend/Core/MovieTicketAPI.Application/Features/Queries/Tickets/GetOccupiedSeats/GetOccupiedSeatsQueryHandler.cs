using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Tickets;
using MovieTicketAPI.Domain.Enums;

namespace MovieTicketAPI.Application.Features.Queries.Tickets.GetOccupiedSeats
{
    public class GetOccupiedSeatsQueryHandler : IRequestHandler<GetOccupiedSeatsQueryRequest, GetOccupiedSeatsQueryResponse>
    {
        private readonly ITicketReadRepository _ticketReadRepository;

        public GetOccupiedSeatsQueryHandler(ITicketReadRepository ticketReadRepository)
        {
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<GetOccupiedSeatsQueryResponse> Handle(GetOccupiedSeatsQueryRequest request, CancellationToken cancellationToken)
        {
            // O seansa ait satılmış biletlerin SADECE koltuk numaralarını çekiyoruz
            var occupiedSeats = await _ticketReadRepository
                .GetWhere(t => t.ShowtimeId == request.ShowtimeId && t.Status == TicketStatus.Active)
                .Select(t => t.SeatNumber)
                .ToListAsync(cancellationToken);

            return new GetOccupiedSeatsQueryResponse
            {
                OccupiedSeats = occupiedSeats
            };
        }
    }
}
