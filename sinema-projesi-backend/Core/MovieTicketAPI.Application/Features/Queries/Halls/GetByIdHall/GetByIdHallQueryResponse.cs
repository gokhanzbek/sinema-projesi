using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.Halls.GetByIdHall
{
    public class GetByIdHallQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }
}
