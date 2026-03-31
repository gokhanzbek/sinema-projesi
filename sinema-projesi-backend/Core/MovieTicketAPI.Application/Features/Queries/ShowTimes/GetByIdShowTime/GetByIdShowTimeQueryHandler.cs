using MediatR;
using MovieTicketAPI.Application.Repositories.Showtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetByIdShowTime
{
    public class GetByIdShowTimeQueryHandler : IRequestHandler<GetByIdShowTimeQueryRequest, GetByIdShowTimeQueryResponse>
    {
        private readonly IShowTimeReadRepository _showTimeReadRepository;

        // Dependency Injection
        public GetByIdShowTimeQueryHandler(IShowTimeReadRepository showTimeReadRepository)
        {
            _showTimeReadRepository = showTimeReadRepository;
        }

        public async Task<GetByIdShowTimeQueryResponse> Handle(GetByIdShowTimeQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. İstenilen ID'ye göre seansı veritabanından çek. (tracking: false çok önemli!)
            var showTime = await _showTimeReadRepository.GetByIdAsync(request.Id.ToString(), tracking: false);

            // 2. Eğer böyle bir seans yoksa null veya boş dönebiliriz (İleride burada "NotFoundException" fırlatabilirsin)
            if (showTime == null)
            {
                return new GetByIdShowTimeQueryResponse(); // Ya da null dönebilirsin
            }

            // 3. Veritabanından gelen Entity nesnesini, Response nesnesine çevir (Mapping) ve yolla
            return new GetByIdShowTimeQueryResponse
            {
                Id = showTime.Id,
                StartTime = showTime.StartTime,
                Price = showTime.Price,
                MovieId = showTime.MovieId,
                HallId = showTime.HallId
            };
        }
    }
}
