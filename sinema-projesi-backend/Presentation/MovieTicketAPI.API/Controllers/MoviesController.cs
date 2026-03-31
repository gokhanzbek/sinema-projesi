using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketAPI.Application.Features.Command.Movie.CreateMovie;
using MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie;
using MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie;
using MovieTicketAPI.Application.Features.Queries.Movies.GetAllMovies;
using MovieTicketAPI.Application.Features.Queries.Movies.GetByIdMovie;

namespace MovieTicketAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoviesController(IMediator mediator)
        {
            _mediator = mediator;
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

        // 3. POST: Yeni film ekle
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMovieCommandRequest request)
        {
            // Body'den (JSON) gelen verileri direkt kuryeye teslim et
            CreateMovieCommandResponse response = await _mediator.Send(request);
            return Ok(response); // İstersen StatusCode(201) de dönebilirsin
        }

        // 4. PUT: Filmi güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateMovieCommandRequest request)
        {
            UpdateMovieCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        // 5. DELETE: Filmi sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            RemoveMovieCommandResponse response = await _mediator.Send(new RemoveMovieCommandRequest { Id = id });
            return Ok(response);
        }
    }
}

