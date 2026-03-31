using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; } // Senin belirlediğin gibi int (Identity)
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // Güncellenmemiş olabilir, o yüzden nullable (?)
    }
}
