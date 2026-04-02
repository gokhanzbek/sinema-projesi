using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieTicketAPI.Application.Features.Command.Tickets.CreateTicket;
using MovieTicketAPI.Application.Features.Command.Tickets.RemoveTicket;
using MovieTicketAPI.Application.Features.Queries.Tickets.GetMyTickets;
using MovieTicketAPI.Application.Features.Queries.Tickets.GetOccupiedSeats;

namespace MovieTicketAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketCommandRequest request)
        {
            CreateTicketCommandResponse response = await _mediator.Send(request);

            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response); // Bir hata olursa örnk yetkisiz.. 400 döner.
        }

        [HttpGet("my-tickets")]

        public async Task<IActionResult> GetMyTickets()
        {
            // Request içi boş olduğu için new'leyip gönderiyoruz
            var request = new GetMyTicketsQueryRequest();
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpGet("occupied-seats/{showtimeId}")]
        [AllowAnonymous] //Authorize bu metodu etkilemez! Herkes görebilir.
        public async Task<IActionResult> GetOccupiedSeats([FromRoute] int showtimeId)
        {
            var request = new GetOccupiedSeatsQueryRequest { ShowtimeId = showtimeId };
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        /// <summary>Kullanıcı kendi aktif biletini iptal eder (durum = İptal, koltuk boşalır).</summary>
        [HttpPut("cancel/{id:int}")]
        public Task<IActionResult> CancelTicketPut([FromRoute] int id) => CancelTicketAsync(id);

        /// <summary>İptal (POST) — Angular ve bazı ortamlar için; PUT ile aynı işlem.</summary>
        [HttpPost("cancel/{id:int}")]
        public Task<IActionResult> CancelTicketPost([FromRoute] int id) => CancelTicketAsync(id);

        private async Task<IActionResult> CancelTicketAsync(int id)
        {
            var request = new RemoveTicketCommandRequest { TicketId = id };
            var response = await _mediator.Send(request);

            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }
    }
}

