using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieTicketAPI.Application.Abstractions.Services;

namespace MovieTicketAPI.Infrastructure.Services.Omdb
{
    public sealed class OmdbMovieRatingService : IOmdbMovieRatingService
    {
        private static int _emptyKeyLogged;

        private readonly HttpClient _httpClient;
        private readonly IOptions<OmdbOptions> _options;
        private readonly ILogger<OmdbMovieRatingService> _logger;

        public OmdbMovieRatingService(
            HttpClient httpClient,
            IOptions<OmdbOptions> options,
            ILogger<OmdbMovieRatingService> logger)
        {
            _httpClient = httpClient;
            _options = options;
            _logger = logger;
        }

        public async Task<decimal?> GetImdbRatingByTitleYearAsync(
            string title,
            int releaseYear,
            string? imdbId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var key = _options.Value.ApiKey?.Trim();
                if (string.IsNullOrEmpty(key))
                {
                    if (Interlocked.Exchange(ref _emptyKeyLogged, 1) == 0)
                    {
                        _logger.LogWarning(
                            "OMDb ApiKey boş. user-secrets: dotnet user-secrets set \"Omdb:ApiKey\" \"ANAHTAR\" (API projesi klasöründe)");
                    }

                    return null;
                }

                var baseUrl = (_options.Value.BaseUrl ?? "https://www.omdbapi.com/").TrimEnd('/');
                var idTrimmed = string.IsNullOrWhiteSpace(imdbId) ? null : imdbId.Trim();

                string url;
                if (idTrimmed != null)
                {
                    url =
                        $"{baseUrl}/?apikey={Uri.EscapeDataString(key)}"  //https://www.omdbapi.com/?apikey=1a2b3c4d&i=tt0111161 gibi görünür çalışınca
                        + $"&i={Uri.EscapeDataString(idTrimmed)}";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(title))
                        return null;

                    url =
                        $"{baseUrl}/?apikey={Uri.EscapeDataString(key)}"
                        + $"&t={Uri.EscapeDataString(title.Trim())}"
                        + $"&y={releaseYear}";
                }

                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OMDb HTTP {Status} — {Reason}", (int)response.StatusCode, response.ReasonPhrase);
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                var root = doc.RootElement;

                if (root.TryGetProperty("Response", out var resp) &&
                    string.Equals(resp.GetString(), "False", StringComparison.OrdinalIgnoreCase))
                {
                    var err = root.TryGetProperty("Error", out var errEl) ? errEl.GetString() : null;
                    _logger.LogWarning("OMDb hata: {Error}", err ?? "Bilinmeyen");
                    return null;
                }

                if (!root.TryGetProperty("imdbRating", out var imdbEl))
                    return null;

                var s = imdbEl.GetString();
                if (string.IsNullOrEmpty(s) || string.Equals(s, "N/A", StringComparison.OrdinalIgnoreCase))
                    return null;
                
                return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                    ? d
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
