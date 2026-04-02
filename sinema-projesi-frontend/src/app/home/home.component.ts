import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, combineLatest, map, Observable, of, startWith, BehaviorSubject, switchMap } from 'rxjs';
import { HomeMovieFilters, Movie } from '../models/movie.models';
import { MovieService } from '../services/movie.service';
import { ImdbRatingComponent } from '../shared/imdb-rating/imdb-rating.component';

type HomeVm = {
  isLoading: boolean;
  errorMessage: string | null;
  movies: Movie[];
};

type ModalDraft = {
  minPrice: number;
  maxPrice: number;
  genres: string[];
  timeSlots: string[];
  sort: string;
};

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ImdbRatingComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  private readonly filters$ = new BehaviorSubject<HomeMovieFilters>(HomeComponent.defaultFilters());

  /** Toolbar + liste için tek abonelik */
  readonly view$: Observable<{ filters: HomeMovieFilters; vm: HomeVm }>;

  modalOpen = false;
  modalDraft: ModalDraft = HomeComponent.defaultModalDraft();

  readonly priceMin = 100;
  readonly priceMax = 280;
  readonly priceStep = 5;

  readonly genreOptions = [
    'Aile',
    'Aksiyon',
    'Animasyon',
    'Bilim Kurgu',
    'Biyografi',
    'Dram',
    'Fantastik',
    'Gerilim',
    'Gizem',
    'Komedi',
    'Korku',
    'Macera',
    'Romantik',
    'Spor',
    'Suç'
  ] as const;

  readonly timeSlotOptions = [
    { id: 'slot12_18', label: '12:00 – 18:00' },
    { id: 'slot18_22', label: '18:00 – 22:00' },
    { id: 'slot22_00', label: '22:00 – 00:00' },
    { id: 'slot00_12', label: '00:00 – 12:00' }
  ] as const;

  readonly sortOptions: { value: string; label: string }[] = [
    { value: 'recommended', label: 'Önerilen' },
    { value: 'imdb_desc', label: 'IMDb puanına göre (yüksekten düşüğe)' },
    { value: 'imdb_asc', label: 'IMDb puanına göre (düşükten yükseğe)' },
    { value: 'title_asc', label: 'A-Z (film adına göre)' },
    { value: 'title_desc', label: 'Z-A (film adına göre)' }
  ];

  constructor(private readonly movieService: MovieService) {
    const vm$ = this.filters$.pipe(
      switchMap((f) =>
        this.movieService.getFilteredMovies(f).pipe(
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
              movies: [] as Movie[]
            })
          ),
          startWith({
            isLoading: true,
            errorMessage: null,
            movies: [] as Movie[]
          })
        )
      )
    );

    this.view$ = combineLatest([this.filters$, vm$]).pipe(
      map(([filters, vm]) => ({ filters, vm }))
    );
  }

  private static defaultFilters(): HomeMovieFilters {
    const t = new Date();
    const y = t.getFullYear();
    const m = String(t.getMonth() + 1).padStart(2, '0');
    const d = String(t.getDate()).padStart(2, '0');
    return {
      eventDate: `${y}-${m}-${d}`,
      sort: 'recommended',
      minPrice: null,
      maxPrice: null,
      genres: [],
      timeSlots: []
    };
  }

  private static defaultModalDraft(): ModalDraft {
    return {
      minPrice: 100,
      maxPrice: 280,
      genres: [],
      timeSlots: [],
      sort: 'recommended'
    };
  }

  onEventDateChange(value: string): void {
    const cur = this.filters$.value;
    this.filters$.next({ ...cur, eventDate: value });
  }

  openFilterModal(): void {
    const f = this.filters$.value;
    this.modalDraft = {
      minPrice: f.minPrice ?? 100,
      maxPrice: f.maxPrice ?? 280,
      genres: [...f.genres],
      timeSlots: [...f.timeSlots],
      sort: f.sort
    };
    this.modalOpen = true;
  }

  closeFilterModal(): void {
    this.modalOpen = false;
  }

  applyFiltersFromModal(): void {
    const cur = this.filters$.value;
    this.filters$.next({
      ...cur,
      minPrice: this.modalDraft.minPrice,
      maxPrice: this.modalDraft.maxPrice,
      genres: [...this.modalDraft.genres],
      timeSlots: [...this.modalDraft.timeSlots],
      sort: this.modalDraft.sort
    });
    this.modalOpen = false;
  }

  clearAllFilters(): void {
    const eventDate = this.filters$.value.eventDate;
    this.filters$.next({
      eventDate,
      sort: 'recommended',
      minPrice: null,
      maxPrice: null,
      genres: [],
      timeSlots: []
    });
    this.modalDraft = HomeComponent.defaultModalDraft();
  }

  toggleGenre(genre: string): void {
    const g = [...this.modalDraft.genres];
    const i = g.indexOf(genre);
    if (i >= 0) {
      g.splice(i, 1);
    } else {
      g.push(genre);
    }
    this.modalDraft = { ...this.modalDraft, genres: g };
  }

  isGenreSelected(genre: string): boolean {
    return this.modalDraft.genres.includes(genre);
  }

  toggleTimeSlot(id: string): void {
    const t = [...this.modalDraft.timeSlots];
    const i = t.indexOf(id);
    if (i >= 0) {
      t.splice(i, 1);
    } else {
      t.push(id);
    }
    this.modalDraft = { ...this.modalDraft, timeSlots: t };
  }

  isTimeSlotSelected(id: string): boolean {
    return this.modalDraft.timeSlots.includes(id);
  }

  onDraftMinPriceInput(value: number | string): void {
    const n = typeof value === 'string' ? Number(value) : value;
    const v = Math.min(n, this.modalDraft.maxPrice);
    this.modalDraft = { ...this.modalDraft, minPrice: v };
  }

  onDraftMaxPriceInput(value: number | string): void {
    const n = typeof value === 'string' ? Number(value) : value;
    const v = Math.max(n, this.modalDraft.minPrice);
    this.modalDraft = { ...this.modalDraft, maxPrice: v };
  }

  trackById(_: number, movie: Movie): string {
    return movie.id;
  }

  onPosterError(event: Event): void {
    const img = event.target as HTMLImageElement | null;
    if (!img) return;
    img.src =
      'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22400%22 height=%22600%22 viewBox=%220 0 400 600%22%3E%3Cdefs%3E%3ClinearGradient id=%22g%22 x1=%220%22 y1=%220%22 x2=%221%22 y2=%221%22%3E%3Cstop stop-color=%22%23131a2a%22/%3E%3Cstop offset=%221%22 stop-color=%22%232b1435%22/%3E%3C/linearGradient%3E%3C/defs%3E%3Crect width=%22400%22 height=%22600%22 fill=%22url(%23g)%22/%3E%3Cpath d=%22M120 230h160v20H120zm0 50h120v20H120zm0 50h160v20H120z%22 fill=%22%23ffffff%22 opacity=%220.35%22/%3E%3C/svg%3E';
  }
}
