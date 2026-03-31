using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Queries.ShowTimes.GetAllShowTimes
{
    public class GetAllShowTimesQueryResponse
    {
        // Birden fazla seans döneceğimiz için, sonuçları bir Liste veya IEnumerable olarak tutuyoruz.
        // Gelen listedeki her bir elemanın tipini anonim bir obje gibi tasarlıyoruz (veya ayrı bir DTO açılabilir).
        public object ShowTimes { get; set; } // En kolay yöntem olarak object verdim, DTO eklersen daha iyi olur.
        public int TotalCount { get; set; } // Kaç tane seans olduğunu dönmek iyi bir standarttır.
    }
}
