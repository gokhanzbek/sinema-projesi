using MediatR;
using System;

namespace MovieTicketAPI.Application.Features.Queries.Movies.GetFilteredMovies
{
    public class GetFilteredMoviesQueryRequest : IRequest<GetFilteredMoviesQueryResponse>
    {
        /// <summary>Etkinlik günü (tarih kısmı kullanılır). Boşsa bugün.</summary>
        public DateTime? EventDate { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        /// <summary>Virgülle ayrılmış tür adları (örn. Aile,Aksiyon). Boş veya atlanabilir.</summary>
        public string? Genres { get; set; }

        /// <summary>Virgülle ayrılmış zaman dilimleri: slot12_18, slot18_22, slot22_00, slot00_12. Boş veya atlanabilir.</summary>
        public string? TimeSlots { get; set; }

        /// <summary>recommended | imdb_desc | imdb_asc | title_asc | title_desc</summary>
        public string? Sort { get; set; } = "recommended";
    }
}
