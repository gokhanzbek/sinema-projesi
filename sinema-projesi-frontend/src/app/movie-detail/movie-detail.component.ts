import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  catchError,
  forkJoin,
  map,
  Observable,
  of,
  startWith,
  switchMap,
  tap
} from 'rxjs';
import { DateShowtimes, MovieDetail } from '../models/movie.models';
import { AuthService } from '../services/auth.service';
import { MovieService } from '../services/movie.service';
import { ImdbRatingComponent } from '../shared/imdb-rating/imdb-rating.component';

type MovieDetailVm = {
  isLoading: boolean;
  errorMessage: string | null;
  movie: MovieDetail | null;
  showtimes: DateShowtimes[];
};

const TR_MONTH_INDEX: Record<string, number> = {
  Ocak: 0,
  Şubat: 1,
  Mart: 2,
  Nisan: 3,
  Mayıs: 4,
  Haziran: 5,
  Temmuz: 6,
  Ağustos: 7,
  Eylül: 8,
  Ekim: 9,
  Kasım: 10,
  Aralık: 11
};

@Component({
  selector: 'app-movie-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ImdbRatingComponent],
  templateUrl: './movie-detail.component.html',
  styleUrl: './movie-detail.component.scss'
})
export class MovieDetailComponent {
  @ViewChild('dateStripViewport') dateStripViewport?: ElementRef<HTMLElement>;

  readonly vm$: Observable<MovieDetailVm>;
  selectedDateTitle: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly auth: AuthService,
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
          tap(({ showtimes }) => {
            if (showtimes && showtimes.length > 0) {
              if (!this.selectedDateTitle) {
                this.selectedDateTitle = showtimes[0].dateTitle;
              }
            } else {
              this.selectedDateTitle = null;
            }
          }),
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

  selectDate(dateTitle: string): void {
    this.selectedDateTitle = dateTitle;
  }

  onSessionClick(showtimeId: number): void {
    const movieId = this.route.snapshot.paramMap.get('id');
    const returnUrl = movieId ? `/movie/${movieId}` : '/';

    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl }
      });
      return;
    }

    this.router.navigate(['/session', showtimeId, 'seats']);
  }

  scrollDateStrip(direction: number): void {
    const el = this.dateStripViewport?.nativeElement;
    if (!el) return;
    el.scrollBy({ left: direction * 220, behavior: 'smooth' });
  }

  formatTabPrimary(dateTitle: string): string {
    const parts = dateTitle.trim().split(/\s+/);
    if (parts.length >= 4 && /^\d{4}$/.test(parts[2])) {
      return `${parts[0]} ${parts[1]} ${parts[3]}`;
    }
    return dateTitle;
  }

  formatCardDateLine(dateTitle: string): string {
    const parts = dateTitle.trim().split(/\s+/);
    if (parts.length >= 4 && /^\d{4}$/.test(parts[2])) {
      return `${parts[0]} ${parts[1]} ${parts[2]}`;
    }
    return dateTitle;
  }

  formatTabRelative(dateTitle: string): string {
    const d = this.parseDateFromTitle(dateTitle);
    if (!d) return '';
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const target = new Date(d.getFullYear(), d.getMonth(), d.getDate());
    const diffDays = Math.round((target.getTime() - today.getTime()) / 86400000);
    if (diffDays === 0) return 'Bugün';
    if (diffDays === 1) return 'Yarın';
    const dow = target.getDay();
    if (dow === 0 || dow === 6) return 'Hafta sonu';
    return '';
  }

  formatHallSubtitle(movie: MovieDetail): string {
    const fromCats = movie.categories?.filter(Boolean).join(', ')?.trim();
    const g = fromCats || movie.genre?.trim();
    return g || 'Gösterim';
  }

  private parseDateFromTitle(dateTitle: string): Date | null {
    const parts = dateTitle.trim().split(/\s+/);
    if (parts.length < 4) return null;
    const day = parseInt(parts[0], 10);
    const monthIdx = TR_MONTH_INDEX[parts[1]];
    const year = parseInt(parts[2], 10);
    if (Number.isNaN(day) || monthIdx === undefined || Number.isNaN(year)) return null;
    return new Date(year, monthIdx, day);
  }

  onPosterError(event: Event): void {
    const img = event.target as HTMLImageElement | null;
    if (!img) return;
    img.src =
      'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22600%22 height=%22900%22 viewBox=%220 0 600 900%22%3E%3Cdefs%3E%3ClinearGradient id=%22g%22 x1=%220%22 y1=%220%22 x2=%221%22 y2=%221%22%3E%3Cstop stop-color=%22%230b1120%22/%3E%3Cstop offset=%221%22 stop-color=%22%23251539%22/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect width=%22600%22 height=%22900%22 fill=%22url(%23g)%22/%3E%3Cpath d=%22M180 330h240v24H180zm0 60h180v24H180zm0 60h240v24H180z%22 fill=%22%23fff%22 opacity=%220.35%22/%3E%3C/svg%3E';
  }
}

