using MovieTicketAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T model); //Yeni bir veri ekler
        Task<bool> AddRangeAsync(List<T> datas); //Listeler halindeki çoklu veriyi tek seferde ekler.
        bool Remove(T model);  //Verilen veriyi siler.
        bool RemoveRange(List<T> datas);
        Task<bool> RemoveAsync(string id);
        bool Update(T model); //Mevcut veriyi günceller.
        Task<int> SaveAsync(); //Yukarıdaki işlemlerin veritabanına kalıcı olarak işlenmesini sağlar. int döner; yani kaç satırın etkilendiğini söyler.
    }
}
