using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetByIdShowTime
{
    public class GetByIdShowTimeQueryRequest : IRequest<GetByIdShowTimeQueryResponse>
    {
        public int Id { get; set; } // Hangi seansın detayları isteniyor?
    }
}
