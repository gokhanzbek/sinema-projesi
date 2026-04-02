using MovieTicketAPI.Domain.Entities.Common;
using System.Collections.Generic;

namespace MovieTicketAPI.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
