import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  catchError,
  forkJoin,
  map,
  Observable,
  of,
  startWith,
  switchMap
} from 'rxjs';
import { DateShowtimes, MovieDetail } from '../models/movie.models';
import { MovieService } from '../services/movie.service';

type MovieDetailVm = {
  isLoading: boolean;
  errorMessage: string | null;
  movie: MovieDetail | null;
  showtimes: DateShowtimes[];
};

@Component({
  selector: 'app-movie-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './movie-detail.component.html',
  styleUrl: './movie-detail.component.scss'
})
export class MovieDetailComponent {
  readonly vm$: Observable<MovieDetailVm>;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly movieService: MovieService
  ) {
    this.vm$ = this.route.paramMap.pipe(
      map((params) => params.get('id')),
      switchMap((id) => {
        if (!id) {
          return of({
            isLoading: false,
            errorMessage: 'Film bulunamadı.',
            movie: null,
            showtimes: []
          });
        }

        return forkJoin({
          movie: this.movieService.getMovieById(id),
          showtimes: this.movieService
            .getShowtimesByMovie(id)
            .pipe(catchError(() => of([] as DateShowtimes[])))
        }).pipe(
          map(({ movie, showtimes }) => ({
            isLoading: false,
            errorMessage: null,
            movie,
            showtimes
          })),
          catchError(() =>
            of({
              isLoading: false,
              errorMessage: 'Film detayları alınamadı. Lütfen tekrar deneyin.',
              movie: null,
              showtimes: []
            })
          )
        );
      }),
      startWith({ isLoading: true, errorMessage: null, movie: null, showtimes: [] })
    );
  }

  formatDuration(duration: string | null): string {
    return duration?.trim() || 'Süre bilgisi yakında';
  }

  formatReleaseDate(releaseDate: string | null): string {
    return releaseDate?.trim() || 'Vizyon tarihi yakında';
  }

  formatGenre(genre: string | null): string {
    return genre?.trim() || 'Tür bilgisi yakında';
  }

  formatDirector(director: string | null): string {
    return director?.trim() || 'Yönetmen bilgisi yakında';
  }

  formatDescription(description: string | null): string {
    return (
      description?.trim() ||
      'Bu filmin açıklaması henüz eklenmedi. Çok yakında detaylı özet burada olacak.'
    );
  }

  formatPrice(price: number): string {
    return `${price.toFixed(0)} TL`;
  }

  onPosterError(event: Event): void {
    const img = event.target as HTMLImageElement | null;
    if (!img) return;
    img.src =
      'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22600%22 height=%22900%22 viewBox=%220 0 600 900%22%3E%3Cdefs%3E%3ClinearGradient id=%22g%22 x1=%220%22 y1=%220%22 x2=%221%22 y2=%221%22%3E%3Cstop stop-color=%22%230b1120%22/%3E%3Cstop offset=%221%22 stop-color=%22%23251539%22/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect width=%22600%22 height=%22900%22 fill=%22url(%23g)%22/%3E%3Cpath d=%22M180 330h240v24H180zm0 60h180v24H180zm0 60h240v24H180z%22 fill=%22%23fff%22 opacity=%220.35%22/%3E%3C/svg%3E';
  }
}

