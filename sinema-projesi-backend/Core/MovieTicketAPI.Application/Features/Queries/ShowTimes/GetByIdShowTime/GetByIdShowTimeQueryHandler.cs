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
            var showTime = await _showTimeReadRepository.GetByIdWithHallAsync(request.Id, cancellationToken);

            if (showTime == null)
            {
                return new GetByIdShowTimeQueryResponse();
            }

            var hall = showTime.Hall;
            return new GetByIdShowTimeQueryResponse
            {
                Id = showTime.Id,
                StartTime = showTime.StartTime,
                Price = showTime.Price,
                MovieId = showTime.MovieId,
                HallId = showTime.HallId,
                HallRowCount = hall?.RowCount ?? 0,
                HallColumnCount = hall?.ColumnCount ?? 0
            };
        }
    }
}
