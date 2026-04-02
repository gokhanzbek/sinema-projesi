using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Application.Repositories.Halls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Halls.GetAllHalls
{
    public class GetAllHallsQueryHandler : IRequestHandler<GetAllHallsQueryRequest, GetAllHallsQueryResponse>
    {
        private readonly IHallReadRepository _hallReadRepository;
        public GetAllHallsQueryHandler(IHallReadRepository hallReadRepository) => _hallReadRepository = hallReadRepository;

        public async Task<GetAllHallsQueryResponse> Handle(GetAllHallsQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _hallReadRepository.GetAll(tracking: false);
            return new GetAllHallsQueryResponse
            {
                TotalCount = await query.CountAsync(),
                Halls = await query.Select(h => new { h.Id, h.Name, h.Capacity, h.RowCount, h.ColumnCount }).ToListAsync()
            };
        }
    }
}
