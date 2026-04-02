using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Halls.UpdateHall
{
    public class UpdateHallCommandRequest : IRequest<UpdateHallCommandResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }     // SalonAdi (nvarchar 50)
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }
}
