using MediatR;
using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Halls.RemoveHall;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.CreateMovie
{
    public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommandRequest, CreateMovieCommandResponse>
    {
        private readonly IMovieWriteRepository _movieWriteRepository;

        public CreateMovieCommandHandler(IMovieWriteRepository movieWriteRepository)
        {
            _movieWriteRepository = movieWriteRepository;
        }

        public async Task<CreateMovieCommandResponse> Handle(CreateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            

            Domain.Entities.Movie newMovie = new Domain.Entities.Movie
            {
                ImageUrl = request.ImageUrl,
                Title = request.Title,
                DurationInMinutes = request.DurationInMinutes,
                Genre = request.Genre,
                Director = request.Director,
                ReleaseYear = request.ReleaseYear,
                Description = request.Description
            };
            await _movieWriteRepository.AddAsync(newMovie);
            await _movieWriteRepository.SaveAsync();

            return new CreateMovieCommandResponse
            {
                Id = newMovie.Id,
                IsSuccess = true,
                Message = "Görev başarıyla eklendi!"
                };
        }
    }
}
