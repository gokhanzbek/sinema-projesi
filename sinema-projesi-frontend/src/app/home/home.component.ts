import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, map, Observable, of, startWith } from 'rxjs';
import { Movie } from '../models/movie.models';
import { MovieService } from '../services/movie.service';

type HomeVm = {
  isLoading: boolean;
  errorMessage: string | null;
  movies: Movie[];
};

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  readonly vm$: Observable<HomeVm>;

  constructor(private readonly movieService: MovieService) {
    this.vm$ = this.movieService.getAllMovies().pipe(
      map((movies) => ({
        isLoading: false,
        errorMessage: null,
        movies: movies.filter((m) => m.id && m.title)
      })),
      catchError(() =>
        of({
          isLoading: false,
          errorMessage:
            'Filmler yüklenirken bir hata oluştu. Backend açık mı ve CORS izinleri doğru mu?',
          movies: []
        })
      ),
      startWith({
        isLoading: true,
        errorMessage: null,
        movies: []
      })
    );
  }

  trackById(_: number, movie: Movie): string {
    return movie.id;
  }

  onPosterError(event: Event): void {
    const img = event.target as HTMLImageElement | null;
    if (!img) return;
    img.src = 'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22400%22 height=%22600%22 viewBox=%220 0 400 600%22%3E%3Cdefs%3E%3ClinearGradient id=%22g%22 x1=%220%22 y1=%220%22 x2=%221%22 y2=%221%22%3E%3Cstop stop-color=%22%23131a2a%22/%3E%3Cstop offset=%221%22 stop-color=%22%232b1435%22/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect width=%22400%22 height=%22600%22 fill=%22url(%23g)%22/%3E%3Cpath d=%22M120 230h160v20H120zm0 50h120v20H120zm0 50h160v20H120z%22 fill=%22%23ffffff%22 opacity=%220.35%22/%3E%3C/svg%3E';
  }
}

