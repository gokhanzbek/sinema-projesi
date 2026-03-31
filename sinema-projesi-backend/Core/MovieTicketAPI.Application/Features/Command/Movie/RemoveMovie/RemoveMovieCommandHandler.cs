using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Halls.RemoveHall;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.RemoveMovie
{
    public class RemoveMovieCommandHandler : IRequestHandler<RemoveMovieCommandRequest, RemoveMovieCommandResponse>
    {
        IMovieWriteRepository _movieWriteRepository;
        IMovieReadRepository _movieReadRepository;

        public RemoveMovieCommandHandler(IMovieWriteRepository movieWriteRepository, IMovieReadRepository movieReadRepository)
        {
            _movieWriteRepository = movieWriteRepository;
            _movieReadRepository = movieReadRepository;
        }

        public async Task<RemoveMovieCommandResponse> Handle(RemoveMovieCommandRequest request, CancellationToken cancellationToken)
        {
            

            Domain.Entities.Movie movie = await _movieReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);
            if (movie == null)
            {
                return new RemoveMovieCommandResponse
                {
                    Message = "film bulunamadı!",
                    IsSuccess = false

                };

            }

            _movieWriteRepository.Remove(movie);
            await _movieWriteRepository.SaveAsync();
            return new RemoveMovieCommandResponse
            {
                IsSuccess = true,
                Message = "Film başarıyla silindi!"
            };


        }
    }
}
