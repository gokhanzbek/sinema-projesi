namespace MovieTicketAPI.Application.Features.Queries.Movies.GetFilteredMovies
{
    public class GetFilteredMoviesQueryResponse
    {
        public int TotalCount { get; set; }
        public object Movies { get; set; }
    }
}
