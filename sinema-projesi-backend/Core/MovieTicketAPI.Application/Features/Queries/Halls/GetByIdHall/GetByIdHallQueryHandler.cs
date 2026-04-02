using MediatR;
using MovieTicketAPI.Application.Repositories.Halls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Halls.GetByIdHall
{
    public class GetByIdHallQueryHandler : IRequestHandler<GetByIdHallQueryRequest, GetByIdHallQueryResponse>
    {
        private readonly IHallReadRepository _hallReadRepository;
        public GetByIdHallQueryHandler(IHallReadRepository hallReadRepository) => _hallReadRepository = hallReadRepository;

        public async Task<GetByIdHallQueryResponse> Handle(GetByIdHallQueryRequest request, CancellationToken cancellationToken)
        {
            var hall = await _hallReadRepository.GetByIdAsync(request.Id.ToString(), tracking: false);
            if (hall == null) return new GetByIdHallQueryResponse();

            return new GetByIdHallQueryResponse
            {
                Id = hall.Id,
                Name = hall.Name,
                Capacity = hall.Capacity,
                RowCount = hall.RowCount,
                ColumnCount = hall.ColumnCount
            };
        }
    }   
}
