namespace MovieTicketAPI.Application.Features.Command.Movie.SyncMoviesImdbRatings
{
    public class SyncMoviesImdbRatingsCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalMovies { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedNoRatingCount { get; set; }
        public int FailedCount { get; set; }
    }
}
