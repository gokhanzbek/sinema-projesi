using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Showtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetAllShowTimes
{
    public class GetAllShowTimesQueryHandler : IRequestHandler<GetAllShowTimesQueryRequest, GetAllShowTimesQueryResponse>
    {
        // SADECE ReadRepository'ye ihtiyacımız var, Write ile işimiz yok!
        private readonly IShowTimeReadRepository _showTimeReadRepository;

        public GetAllShowTimesQueryHandler(IShowTimeReadRepository showTimeReadRepository)
        {
            _showTimeReadRepository = showTimeReadRepository;
        }

        public async Task<GetAllShowTimesQueryResponse> Handle(GetAllShowTimesQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. Tüm seansları getir. 'tracking: false' performansı roket gibi yapar!
            var query = _showTimeReadRepository.GetAll(tracking: false);

            // 2. Toplam seans sayısını al
            int totalCount = await query.CountAsync();

            // 3. İhtiyacımız olan alanları seç (Select - Projection) ve listeye çevir
            var showTimes = await query.Select(s => new
            {
                s.Id,
                MovieName = s.Movie.Title,
                HallName = s.Hall.Name,
                s.StartTime
            }).ToListAsync();

            // 4. Hazırladığımız verileri Response nesnesine koy ve yolla
            return new GetAllShowTimesQueryResponse
            {
                TotalCount = totalCount,
                ShowTimes = showTimes
            };
        }
    }
}
