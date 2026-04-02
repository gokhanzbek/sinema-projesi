namespace MovieTicketAPI.Infrastructure.Services.Omdb
{
    public class OmdbOptions
    {
        public const string SectionName = "Omdb";

        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Varsayılan: https://www.omdbapi.com/</summary>
        public string BaseUrl { get; set; } = "https://www.omdbapi.com/";
    }
}
