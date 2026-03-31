using MediatR;
using MovieTicketAPI.Application.Abstractions.Services;
using MovieTicketAPI.Application.Features.Command.Halls.UpdateHall;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application.Features.Command.Movie.UpdateMovie
{
    public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommandRequest, UpdateMovieCommandResponse>
    {
        private readonly IMovieReadRepository _movieReadRepository;
        private readonly IMovieWriteRepository _movieWriteRepository;

        public UpdateMovieCommandHandler(IMovieReadRepository movieReadRepository, IMovieWriteRepository movieWriteRepository)
        {
            _movieReadRepository = movieReadRepository;
            _movieWriteRepository = movieWriteRepository;
        }

        async Task<UpdateMovieCommandResponse> IRequestHandler<UpdateMovieCommandRequest, UpdateMovieCommandResponse>.Handle(UpdateMovieCommandRequest request, CancellationToken cancellationToken)
        {

            

            Domain.Entities.Movie movie = await _movieReadRepository.GetByIdAsync(request.Id.ToString(), tracking: true);

            if (movie == null)
            {
                return new UpdateMovieCommandResponse { IsSuccess = false, Message = "Aradığınız Film bulunamadı!" };
            }

            movie.ImageUrl = request.ImageUrl;
            movie.Title = request.Title;
            movie.Director = request.Director;
            movie.DurationInMinutes = request.DurationInMinutes;
            movie.Description = request.Description;
            movie.Genre = request.Genre;
            movie.ReleaseYear = request.ReleaseYear;

            _movieWriteRepository.Update(movie);
            await _movieWriteRepository.SaveAsync();

            return new UpdateMovieCommandResponse { IsSuccess = true, Message = "Film Başarıyla Güncellendi!" };

        }
    }
}
