using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Showtimes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetShowtimesByMovie
{
    public class GetShowtimesByMovieQueryHandler : IRequestHandler<GetShowtimesByMovieQueryRequest, GetShowtimesByMovieQueryResponse>
    {
        private readonly IShowTimeReadRepository _showtimeReadRepository;

        public GetShowtimesByMovieQueryHandler(IShowTimeReadRepository showtimeReadRepository)
        {
            _showtimeReadRepository = showtimeReadRepository;
        }

        public async Task<GetShowtimesByMovieQueryResponse> Handle(GetShowtimesByMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var currentTime = DateTime.UtcNow;

            // 1. Veritabanından filme ait gelecek seansları, Salon (Hall) bilgisiyle çek
            var showtimes = await _showtimeReadRepository
                .GetWhere(s => s.MovieId == request.MovieId && s.StartTime > currentTime)
                .Include(s => s.Hall) // Salon adını ("Salon 1" vb.) çekebilmek için ekledik
                .ToListAsync(cancellationToken);

            // 2. Biletinial tarzı Gruplama (Tarih -> Salon -> Saatler)
            var groupedShowtimes = showtimes
                .GroupBy(s => s.StartTime.Date) // Önce Günlere böl (Klasör 1)
                .OrderBy(dateGroup => dateGroup.Key)
                .Select(dateGroup => new
                {
                    // Örn: "27 Mart 2026 Cuma"
                    DateTitle = dateGroup.Key.ToString("dd MMMM yyyy dddd", new CultureInfo("tr-TR")),

                    Halls = dateGroup
                        .GroupBy(s => s.Hall.Name) // Sonra o günkü Salonlara böl (Klasör 2)
                        .Select(hallGroup => new
                        {
                            HallName = hallGroup.Key, // Örn: "Salon 1"

                            Times = hallGroup
                                .OrderBy(s => s.StartTime) // Saatleri sabahtan akşama diz
                                .Select(s => new
                                {
                                    ShowtimeId = s.Id,
                                    Time = s.StartTime.ToString("HH:mm"), // Sadece saat ve dakika (Örn: "14:30")
                                    Price = s.Price
                                }).ToList()
                        }).ToList()
                }).ToList();

            return new GetShowtimesByMovieQueryResponse
            {
                Showtimes = groupedShowtimes
            };
        }
    }
}

