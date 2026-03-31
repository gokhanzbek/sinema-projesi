import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import {
  DateShowtimes,
  GetAllMoviesResponse,
  GetMovieByIdResponse,
  GetShowtimesByMovieResponse,
  Movie,
  MovieDetail
} from '../models/movie.models';

@Injectable({ providedIn: 'root' })
export class MovieService {
  private readonly baseUrl = 'http://localhost:5267/api/Movies';

  constructor(private readonly http: HttpClient) {}

  getAllMovies(): Observable<Movie[]> {
    return this.http.get<GetAllMoviesResponse>(this.baseUrl).pipe(
      map((res) => {
        const movies = Array.isArray(res?.movies) ? res.movies : [];
        return movies.map((m) => ({
          id: (m.id ?? m.Id ?? '').toString(),
          title: (m.title ?? m.Title ?? '').toString(),
          imageUrl: (m.imageUrl ?? m.ImageUrl ?? null) as string | null
        }));
      })
    );
  }

  getMovieById(id: string | number): Observable<MovieDetail> {
    return this.http.get<GetMovieByIdResponse>(`${this.baseUrl}/${id}`).pipe(
      map((res) => ({
        id: (res.id ?? res.Id ?? '').toString(),
        title: (res.title ?? res.Title ?? '').toString(),
        imageUrl: (res.imageUrl ?? res.ImageUrl ?? null) as string | null,
        description: (res.description ?? res.Description ?? null) as string | null,
        duration:
          (res.duration ??
            res.Duration ??
            (res.durationInMinutes ?? res.DurationInMinutes)
              ?.toString()) as string | null,
        director: (res.director ?? res.Director ?? null) as string | null,
        genre: (res.genre ?? res.Genre ?? null) as string | null,
        releaseDate:
          (res.releaseDate ??
            res.ReleaseDate ??
            (res.releaseYear ?? res.ReleaseYear)?.toString()) as string | null
      }))
    );
  }

  getShowtimesByMovie(movieId: string | number): Observable<DateShowtimes[]> {
    return this.http
      .get<GetShowtimesByMovieResponse>(
        `http://localhost:5267/api/ShowTimes/by-movie/${movieId}`
      )
      .pipe(
        map((res) => {
          const dateGroups = Array.isArray(res?.showtimes) ? res.showtimes : [];
          return dateGroups.map((dateGroup) => {
            const halls = Array.isArray(dateGroup.halls ?? dateGroup.Halls)
              ? (dateGroup.halls ?? dateGroup.Halls)!
              : [];
            return {
              dateTitle: (dateGroup.dateTitle ?? dateGroup.DateTitle ?? '').toString(),
              halls: halls.map((hall) => {
                const times = Array.isArray(hall.times ?? hall.Times)
                  ? (hall.times ?? hall.Times)!
                  : [];
                return {
                  hallName: (hall.hallName ?? hall.HallName ?? '').toString(),
                  times: times.map((time) => ({
                    showtimeId: Number(time.showtimeId ?? time.ShowtimeId ?? 0),
                    time: (time.time ?? time.Time ?? '').toString(),
                    price: Number(time.price ?? time.Price ?? 0)
                  }))
                };
              })
            };
          });
        })
      );
  }
}

