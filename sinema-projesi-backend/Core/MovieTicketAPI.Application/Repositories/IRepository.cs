using Microsoft.EntityFrameworkCore;
using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {//where T : BaseEntity= T sadece BaseEntity'den türemiş class olabilir.
        DbSet<T> Table { get; }
    }
}
