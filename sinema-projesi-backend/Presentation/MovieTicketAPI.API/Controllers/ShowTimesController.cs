using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieTicketAPI.Application.Features.Command.ShowTimes.CreateShowTimes;
using MovieTicketAPI.Application.Features.Command.ShowTimes.RemoveShowTimes;
using MovieTicketAPI.Application.Features.Command.ShowTimes.UpdateShowTimes;
using MovieTicketAPI.Application.Features.Queries.ShowTimes.GetAllShowTimes;
using MovieTicketAPI.Application.Features.Queries.ShowTimes.GetByIdShowTime;
using MovieTicketAPI.Application.Features.Queries.ShowTimes.GetShowtimesByMovie;

namespace MovieTicketAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShowTimesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShowTimesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllShowTimesQueryRequest());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await _mediator.Send(new GetByIdShowTimeQueryRequest { Id = id });
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateShowTimeCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateShowTimeCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = await _mediator.Send(new RemoveShowTimeCommandRequest { Id = id });
            return Ok(response);
        }

        [HttpGet("by-movie/{movieId}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetShowtimesByMovie([FromRoute] int movieId)
        {
            var request = new GetShowtimesByMovieQueryRequest { MovieId = movieId };
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}
