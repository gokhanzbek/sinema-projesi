import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { swalDark } from '../core/swal-dark-theme';
import {
  AdminMovieRow,
  CreateMoviePayload,
  MovieService,
  UpdateMoviePayload
} from '../services/movie.service';
import {
  AdminHallRow,
  HallPayload,
  HallService,
  UpdateHallPayload
} from '../services/hall.service';
import {
  AdminSessionRow,
  SessionService,
  UpdateSessionPayload
} from '../services/session.service';

export type AdminTab = 'movies' | 'halls' | 'sessions';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  private readonly movieService = inject(MovieService);
  private readonly hallService = inject(HallService);
  private readonly sessionService = inject(SessionService);
  private readonly fb = inject(FormBuilder);

  activeTab: AdminTab = 'movies';

  movies: AdminMovieRow[] = [];
  halls: AdminHallRow[] = [];
  sessions: AdminSessionRow[] = [];

  /** API /Categories */
  categoryOptions: { id: number; name: string }[] = [];

  movieModalOpen = false;
  movieEditId: number | null = null;
  posterUploading = false;
  imdbSyncing = false;

  hallModalOpen = false;
  hallEditId: number | null = null;

  sessionModalOpen = false;
  sessionEditId: number | null = null;

  readonly movieForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    durationInMinutes: [120, [Validators.required, Validators.min(1)]],
    categoryIds: this.fb.nonNullable.control<number[]>([]),
    director: [''],
    releaseYear: [new Date().getFullYear(), [Validators.required, Validators.min(1900)]],
    description: [''],
    imageUrl: [''],
    imdbId: [
      '',
      [Validators.maxLength(20), Validators.pattern(/^$|^tt\d+$/i)]
    ]
  });

  readonly hallForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    rowCount: [10, [Validators.required, Validators.min(1), Validators.max(26)]],
    columnCount: [12, [Validators.required, Validators.min(1), Validators.max(40)]]
  });

  readonly sessionForm = this.fb.nonNullable.group({
    movieId: [0, [Validators.required, Validators.min(1)]],
    hallId: [0, [Validators.required, Validators.min(1)]],
    price: [0, [Validators.required, Validators.min(0)]],
    startTime: ['', Validators.required]
  });

  ngOnInit(): void {
    this.movieService.getCategories().subscribe({
      next: (c) => {
        this.categoryOptions = c;
      },
      error: () => {
        this.categoryOptions = [];
      }
    });
    this.reloadLists();
  }

  reloadLists(): void {
    forkJoin({
      movies: this.movieService.getMoviesForAdmin(),
      halls: this.hallService.getAllHalls(),
      sessions: this.sessionService.getAllSessions()
    }).subscribe({
      next: (res) => {
        this.movies = res.movies;
        this.halls = res.halls;
        this.sessions = res.sessions;
      },
      error: () => {
        this.movies = [];
        this.halls = [];
        this.sessions = [];
      }
    });
  }

  setTab(tab: AdminTab): void {
    this.activeTab = tab;
  }

  isTab(tab: AdminTab): boolean {
    return this.activeTab === tab;
  }

  syncImdbRatings(): void {
    if (this.imdbSyncing) return;
    this.imdbSyncing = true;
    this.movieService.syncImdbRatings().subscribe({
      next: (r) => {
        this.imdbSyncing = false;
        if (r.succeeded) {
          this.reloadLists();
          void swalDark.fire({
            icon: 'success',
            title: 'IMDb senkronu',
            html: `<p>${r.message ?? 'Tamamlandı.'}</p>
              <p style="margin-top:8px;font-size:13px;opacity:0.9">
                Toplam: ${r.totalMovies} · Güncellenen: ${r.updatedCount} · Atlanan (puan yok): ${r.skippedNoRatingCount}
                ${r.failedCount ? ` · Hata: ${r.failedCount}` : ''}
              </p>`,
            confirmButtonText: 'Tamam',
            showCancelButton: false
          });
        } else {
          void swalDark.fire({
            icon: 'error',
            title: 'Senkron başarısız',
            text: r.message ?? 'İşlem tamamlanamadı.',
            confirmButtonText: 'Tamam',
            showCancelButton: false
          });
        }
      },
      error: () => {
        this.imdbSyncing = false;
        void swalDark.fire({
          icon: 'error',
          title: 'Hata',
          text: 'Sunucuya ulaşılamadı.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  // —— Film ——
  openMovieCreate(): void {
    this.movieEditId = null;
    this.movieForm.reset({
      title: '',
      durationInMinutes: 120,
      categoryIds: [],
      director: '',
      releaseYear: new Date().getFullYear(),
      description: '',
      imageUrl: '',
      imdbId: ''
    });
    this.movieModalOpen = true;
  }

  openMovieEdit(m: AdminMovieRow): void {
    this.movieEditId = m.id;
    this.movieService.getMovieById(m.id).subscribe({
      next: (d) => {
        this.movieForm.patchValue({
          title: d.title,
          durationInMinutes: d.durationMinutes ?? 120,
          categoryIds: d.categoryIds?.length ? [...d.categoryIds] : [],
          director: d.director ?? '',
          releaseYear: d.releaseYear ?? new Date().getFullYear(),
          description: d.description ?? '',
          imageUrl: d.imageUrl ?? '',
          imdbId: d.imdbId ?? ''
        });
        this.movieModalOpen = true;
      },
      error: () => {
        void swalDark.fire({
          icon: 'error',
          title: 'Yüklenemedi',
          text: 'Film bilgisi alınamadı.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  closeMovieModal(): void {
    this.movieModalOpen = false;
    this.movieEditId = null;
    this.posterUploading = false;
  }

  onPosterFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.posterUploading = true;
    this.movieService.uploadMoviePoster(file).subscribe({
      next: ({ url }) => {
        this.posterUploading = false;
        this.movieForm.patchValue({ imageUrl: url });
        input.value = '';
        void swalDark.fire({
          icon: 'success',
          title: 'Afiş yüklendi',
          text: 'URL forma yazıldı; filmi kaydedin.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      },
      error: (err: Error) => {
        this.posterUploading = false;
        input.value = '';
        void swalDark.fire({
          icon: 'error',
          title: 'Yükleme hatası',
          text: err.message,
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  saveMovie(): void {
    if (this.movieForm.invalid) {
      this.movieForm.markAllAsTouched();
      return;
    }
    const v = this.movieForm.getRawValue();
    if (!v.categoryIds?.length) {
      void swalDark.fire({
        icon: 'warning',
        title: 'Kategori',
        text: 'En az bir kategori seçin.',
        confirmButtonText: 'Tamam',
        showCancelButton: false
      });
      return;
    }
    const imdbTrim = (v.imdbId ?? '').trim();
    const payload: CreateMoviePayload = {
      title: v.title,
      durationInMinutes: v.durationInMinutes,
      categoryIds: v.categoryIds,
      director: v.director,
      releaseYear: v.releaseYear,
      description: v.description,
      imageUrl: v.imageUrl,
      imdbId: imdbTrim.length > 0 ? imdbTrim : null
    };

    const isCreate = this.movieEditId == null;
    const req = isCreate
      ? this.movieService.createMovie(payload)
      : this.movieService.updateMovie({
          ...payload,
          id: this.movieEditId as number
        } as UpdateMoviePayload);

    req.subscribe((out) => {
      if (out.succeeded) {
        this.closeMovieModal();
        this.reloadLists();
        let detail = out.message ?? 'Film kaydedildi.';
        if (isCreate && out.imdbRating != null && out.imdbRating !== undefined) {
          detail += ` IMDb: ${out.imdbRating}`;
        }
        void swalDark.fire({
          icon: 'success',
          title: 'Kaydedildi',
          text: detail,
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Hata',
          text: out.message ?? 'İşlem başarısız.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  async deleteMovie(m: AdminMovieRow): Promise<void> {
    const c = await swalDark.fire({
      title: 'Filmi sil?',
      text: `"${m.title}" kalıcı olarak silinecek.`,
      icon: 'warning',
      showCancelButton: true,
      focusCancel: true,
      confirmButtonText: 'Evet, sil',
      cancelButtonText: 'Vazgeç'
    });
    if (!c.isConfirmed) return;
    this.movieService.deleteMovie(m.id).subscribe((out) => {
      if (out.succeeded) {
        this.reloadLists();
        void swalDark.fire({
          icon: 'success',
          title: 'Silindi',
          text: out.message ?? 'Film silindi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Silinemedi',
          text: out.message ?? '',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  // —— Salon ——
  openHallCreate(): void {
    this.hallEditId = null;
    this.hallForm.reset({ name: '', rowCount: 10, columnCount: 12 });
    this.hallModalOpen = true;
  }

  openHallEdit(h: AdminHallRow): void {
    this.hallEditId = h.id;
    const r = h.rowCount > 0 ? h.rowCount : 10;
    const c = h.columnCount > 0 ? h.columnCount : 12;
    this.hallForm.patchValue({ name: h.name, rowCount: r, columnCount: c });
    this.hallModalOpen = true;
  }

  hallPreviewTotal(): number {
    const v = this.hallForm.getRawValue();
    const rows = Number(v.rowCount);
    const cols = Number(v.columnCount);
    if (!Number.isFinite(rows) || !Number.isFinite(cols)) return 0;
    return Math.max(0, Math.floor(rows)) * Math.max(0, Math.floor(cols));
  }

  closeHallModal(): void {
    this.hallModalOpen = false;
    this.hallEditId = null;
  }

  saveHall(): void {
    if (this.hallForm.invalid) {
      this.hallForm.markAllAsTouched();
      return;
    }
    const v = this.hallForm.getRawValue();
    const base: HallPayload = {
      name: v.name,
      rowCount: v.rowCount,
      columnCount: v.columnCount
    };
    const req =
      this.hallEditId == null
        ? this.hallService.createHall(base)
        : this.hallService.updateHall({ ...base, id: this.hallEditId });

    req.subscribe((out) => {
      if (out.succeeded) {
        this.closeHallModal();
        this.reloadLists();
        void swalDark.fire({
          icon: 'success',
          title: 'Kaydedildi',
          text: out.message ?? 'Salon kaydedildi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Hata',
          text: out.message ?? '',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  async deleteHall(h: AdminHallRow): Promise<void> {
    const c = await swalDark.fire({
      title: 'Salonu sil?',
      text: `"${h.name}" silinecek.`,
      icon: 'warning',
      showCancelButton: true,
      focusCancel: true,
      confirmButtonText: 'Evet, sil',
      cancelButtonText: 'Vazgeç'
    });
    if (!c.isConfirmed) return;
    this.hallService.deleteHall(h.id).subscribe((out) => {
      if (out.succeeded) {
        this.reloadLists();
        void swalDark.fire({
          icon: 'success',
          title: 'Silindi',
          text: out.message ?? 'Salon silindi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Silinemedi',
          text: out.message ?? '',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  // —— Seans ——
  openSessionCreate(): void {
    this.sessionEditId = null;
    const firstMovie = this.movies[0]?.id ?? 0;
    const firstHall = this.halls[0]?.id ?? 0;
    this.sessionForm.reset({
      movieId: firstMovie,
      hallId: firstHall,
      price: 0,
      startTime: this.defaultDatetimeLocal()
    });
    this.sessionModalOpen = true;
  }

  openSessionEdit(s: AdminSessionRow): void {
    this.sessionEditId = s.id;
    this.sessionService.getSessionById(s.id).subscribe({
      next: (d) => {
        if (!d) {
          void swalDark.fire({
            icon: 'error',
            title: 'Bulunamadı',
            text: 'Seans bilgisi alınamadı.',
            confirmButtonText: 'Tamam',
            showCancelButton: false
          });
          return;
        }
        this.sessionForm.patchValue({
          movieId: d.movieId,
          hallId: d.hallId,
          price: d.price,
          startTime: this.toDatetimeLocalValue(d.startTime)
        });
        this.sessionModalOpen = true;
      },
      error: () => {
        void swalDark.fire({
          icon: 'error',
          title: 'Hata',
          text: 'Seans yüklenemedi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  closeSessionModal(): void {
    this.sessionModalOpen = false;
    this.sessionEditId = null;
  }

  saveSession(): void {
    if (this.sessionForm.invalid) {
      this.sessionForm.markAllAsTouched();
      return;
    }
    const v = this.sessionForm.getRawValue();
    const startIso = new Date(v.startTime).toISOString();
    const base = {
      movieId: v.movieId,
      hallId: v.hallId,
      price: v.price,
      startTime: startIso
    };

    const req =
      this.sessionEditId == null
        ? this.sessionService.createSession(base)
        : this.sessionService.updateSession({
            ...base,
            id: this.sessionEditId as number
          } as UpdateSessionPayload);

    req.subscribe((out) => {
      if (out.succeeded) {
        this.closeSessionModal();
        this.reloadLists();
        void swalDark.fire({
          icon: 'success',
          title: 'Kaydedildi',
          text: out.message ?? 'Seans kaydedildi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Hata',
          text: out.message ?? '',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  async deleteSession(s: AdminSessionRow): Promise<void> {
    const c = await swalDark.fire({
      title: 'Seansı sil?',
      text: `${s.movieName} — ${s.date} ${s.time}`,
      icon: 'warning',
      showCancelButton: true,
      focusCancel: true,
      confirmButtonText: 'Evet, sil',
      cancelButtonText: 'Vazgeç'
    });
    if (!c.isConfirmed) return;
    this.sessionService.deleteSession(s.id).subscribe((out) => {
      if (out.succeeded) {
        this.reloadLists();
        void swalDark.fire({
          icon: 'success',
          title: 'Silindi',
          text: out.message ?? 'Seans silindi.',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      } else {
        void swalDark.fire({
          icon: 'error',
          title: 'Silinemedi',
          text: out.message ?? '',
          confirmButtonText: 'Tamam',
          showCancelButton: false
        });
      }
    });
  }

  private defaultDatetimeLocal(): string {
    const d = new Date();
    d.setMinutes(0, 0, 0);
    d.setHours(d.getHours() + 1);
    return this.toDatetimeLocalValue(d.toISOString());
  }

  private toDatetimeLocalValue(iso: string): string {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '';
    const p = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
  }

  toggleMovieCategory(id: number): void {
    const cur = [...(this.movieForm.get('categoryIds')?.value ?? [])];
    const i = cur.indexOf(id);
    if (i >= 0) {
      cur.splice(i, 1);
    } else {
      cur.push(id);
    }
    this.movieForm.patchValue({ categoryIds: cur });
  }

  isMovieCategorySelected(id: number): boolean {
    return (this.movieForm.get('categoryIds')?.value ?? []).includes(id);
  }

  trackMovie = (_: number, m: AdminMovieRow) => m.id;
  trackHall = (_: number, h: AdminHallRow) => h.id;
  trackSession = (_: number, s: AdminSessionRow) => s.id;
}
