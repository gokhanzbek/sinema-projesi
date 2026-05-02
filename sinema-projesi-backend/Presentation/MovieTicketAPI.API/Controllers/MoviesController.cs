using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MovieTicketAPI.Application.Features.Command.Movie.CreateMovie;
using MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie;
using MovieTicketAPI.Application.Features.Command.Movie.SyncMoviesImdbRatings;
using MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie;
using MovieTicketAPI.Application.Features.Queries.Movies.GetAllMovies;
using MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie;
using MovieTicketAPI.Application.Features.Queries.Movies.GetFilteredMovies;

namespace MovieTicketAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private const long MaxPosterBytes = 5_242_880; // 5 MB

        private static readonly HashSet<string> AllowedPosterContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp", "image/gif"
        };

        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public MoviesController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        // 0. GET: Seans / tür / fiyat / zaman dilimine göre filtrelenmiş filmler (Showtimes üzerinden)
        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiltered([FromQuery] GetFilteredMoviesQueryRequest request)
        {
            GetFilteredMoviesQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        // 1. GET: Bütün filmleri listele
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            // Sadece boş bir request gönderiyoruz, kurye (MediatR) gidip Handler'ı bulup çalıştırıyor.
            GetAllMoviesQueryResponse response = await _mediator.Send(new GetAllMoviesQueryRequest());
            return Ok(response);
        }

        // 2. GET: ID'ye göre tek bir film getir
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // Rotadan gelen ID'yi request nesnesine koyup kuryeye veriyoruz.
            GetByIdMovieQueryResponse response = await _mediator.Send(new GetByIdMovieQueryRequest { Id = id });
            return Ok(response);
        }

        /// <summary>Admin: afiş dosyası yükler; dönen tam URL <c>ImageUrl</c> olarak kaydedilir.</summary>
        [HttpPost("upload-poster")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxPosterBytes)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadPoster([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi." });

            if (file.Length > MaxPosterBytes)
                return BadRequest(new { message = "Dosya en fazla 5 MB olabilir." });

            if (string.IsNullOrEmpty(file.ContentType) || !AllowedPosterContentTypes.Contains(file.ContentType))
                return BadRequest(new { message = "Geçersiz dosya türü. JPEG, PNG, WebP veya GIF yükleyin." });

            var ext = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => null
            };
            if (ext == null)
                return BadRequest(new { message = "Geçersiz dosya türü." });

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
                return StatusCode(500, new { message = "Web root yapılandırılmadı." });

            var dir = Path.Combine(webRoot, "uploads", "movies");
            Directory.CreateDirectory(dir);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var physicalPath = Path.Combine(dir, fileName);

            await using (var stream = System.IO.File.Create(physicalPath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var pathBase = Request.PathBase.HasValue ? Request.PathBase.Value! : string.Empty;
            var relative = $"/uploads/movies/{fileName}";
            var url = $"{Request.Scheme}://{Request.Host}{pathBase}{relative}";

            return Ok(new { url });
        }

        /// <summary>Admin: tüm filmlerin IMDb puanını OMDb üzerinden günceller (tek seferlik toplu işlem).</summary>
        [HttpPost("sync-imdb-ratings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncImdbRatings(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SyncMoviesImdbRatingsCommandRequest(), cancellationToken);
            return Ok(response);
        }

        // 3. POST: Yeni film ekle
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateMovieCommandRequest request)
        {
            // Body'den (JSON) gelen verileri direkt kuryeye teslim et
            CreateMovieCommandResponse response = await _mediator.Send(request);
            return Ok(response); // İstersen StatusCode(201) de dönebilirsin
        }

        // 4. PUT: Filmi güncelle
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateMovieCommandRequest request)
        {
            UpdateMovieCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        // 5. DELETE: Filmi sil
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            RemoveMovieCommandResponse response = await _mediator.Send(new RemoveMovieCommandRequest { Id = id });
            return Ok(response);
        }
    }
}

