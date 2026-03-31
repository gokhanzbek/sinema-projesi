using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetAllShowTimes
{
    public class GetAllShowTimesQueryRequest : IRequest<GetAllShowTimesQueryResponse>
    {
        // Şimdilik boş. İleride buraya Sayfalama (Pagination) için "Page" ve "Size" eklenebilir.
    }
}
