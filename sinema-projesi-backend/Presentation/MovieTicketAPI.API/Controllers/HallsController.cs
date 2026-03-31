using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieTicketAPI.Application.Features.Command.Halls.CreateHall;
using MovieTicketAPI.Application.Features.Command.Halls.RemoveHall;
using MovieTicketAPI.Application.Features.Command.Halls.UpdateHall;
using MovieTicketAPI.Application.Features.Queries.Halls.GetAllHalls;
using MovieTicketAPI.Application.Features.Queries.Halls.GetByIdHall;

namespace MovieTicketAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HallsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HallsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET: Tüm salonları listele
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllHallsQueryRequest());
            return Ok(response);
        }

        // 2. GET: ID'ye göre tek bir salon getir
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await _mediator.Send(new GetByIdHallQueryRequest { Id = id });
            return Ok(response);
        }

        // 3. POST: Yeni salon ekle
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHallCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        // 4. PUT: Salon güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateHallCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        // 5. DELETE: Salon sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = await _mediator.Send(new RemoveHallCommandRequest { Id = id });
            return Ok(response);
        }
    }
}
