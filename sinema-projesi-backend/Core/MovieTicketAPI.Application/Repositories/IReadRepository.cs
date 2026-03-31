using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Repositories
{
    public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll(bool tracking = true);//Tablodaki tüm verileri getirir.
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true); //Belirli bir şarta uyanları getirir. 
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);//Şarta uyan tek bir kaydı getirir.
        Task<T> GetByIdAsync(string id, bool tracking = true);//ID'si bilinen özel bir kaydı getirir.
    }

    
}
