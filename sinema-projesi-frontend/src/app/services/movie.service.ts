import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';
import {
  DateShowtimes,
  GetAllMoviesResponse,
  GetMovieByIdResponse,
  GetShowtimesByMovieResponse,
  HomeMovieFilters,
  Movie,
  MovieDetail
} from '../models/movie.models';

function parseImdbRating(raw: unknown): number | null {
  if (raw == null) return null;
  if (typeof raw === 'number' && Number.isFinite(raw)) return raw;
  const s = String(raw).trim().replace(',', '.');
  const n = Number(s);
  return Number.isFinite(n) ? n : null;
}

/** Admin tablosu satırı */
export interface AdminMovieRow {
  id: number;
  title: string;
  categoriesLabel: string;
  posterUrl: string | null;
}

export interface MovieMutationResult {
  succeeded: boolean;
  message: string | null;
  id?: number;
  /** OMDb’den geldiyse film oluşturma yanıtında */
  imdbRating?: number | null;
}

export interface CreateMoviePayload {
  title: string;
  durationInMinutes: number;
  categoryIds: number[];
  director: string;
  releaseYear: number;
  description: string;
  imageUrl: string;
  imdbId?: string | null;
}

export interface UpdateMoviePayload extends CreateMoviePayload {
  id: number;
}

export interface SyncImdbRatingsResult {
  succeeded: boolean;
  message: string | null;
  totalMovies: number;
  updatedCount: number;
  skippedNoRatingCount: number;
  failedCount: number;
}

@Injectable({ providedIn: 'root' })
export class MovieService {
  private readonly baseUrl = `${API_BASE_URL}/Movies`;

  constructor(private readonly http: HttpClient) {}

  getCategories(): Observable<{ id: number; name: string }[]> {
    return this.http
      .get<{ categories?: { id: number; name: string }[]; Categories?: { id: number; name: string }[] }>(
        `${API_BASE_URL}/Categories`
      )
      .pipe(
        map((res) => {
          const raw = res?.categories ?? res?.Categories ?? [];
          return Array.isArray(raw)
            ? raw.map((c) => ({
                id: Number((c as { id?: number; Id?: number }).id ?? (c as { Id?: number }).Id ?? 0),
                name: String((c as { name?: string; Name?: string }).name ?? (c as { Name?: string }).Name ?? '')
              }))
            : [];
        })
      );
  }

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

  /** Seans / fiyat / tür / zaman dilimine göre filtrelenmiş filmler (Showtimes üzerinden). */
  getFilteredMovies(filters: HomeMovieFilters): Observable<Movie[]> {
    let params = new HttpParams()
      .set('eventDate', filters.eventDate)
      .set('sort', filters.sort);
    if (filters.minPrice != null && filters.maxPrice != null) {
      params = params
        .set('minPrice', String(filters.minPrice))
        .set('maxPrice', String(filters.maxPrice));
    }
    if (filters.genres.length > 0) {
      params = params.set('genres', filters.genres.join(','));
    }
    if (filters.timeSlots.length > 0) {
      params = params.set('timeSlots', filters.timeSlots.join(','));
    }
    return this.http
      .get<GetAllMoviesResponse>(`${this.baseUrl}/filter`, { params })
      .pipe(
        map((res) => {
          const movies = Array.isArray(res?.movies) ? res.movies : [];
          return movies.map((m) => ({
            id: (m.id ?? m.Id ?? '').toString(),
            title: (m.title ?? m.Title ?? '').toString(),
            imageUrl: (m.imageUrl ?? m.ImageUrl ?? null) as string | null,
            imdbRating: parseImdbRating(
              (m as { imdbRating?: unknown; ImdbRating?: unknown }).imdbRating ??
                (m as { ImdbRating?: unknown }).ImdbRating
            )
          }));
        })
      );
  }

  getMovieById(id: string | number): Observable<MovieDetail> {
    return this.http.get<GetMovieByIdResponse>(`${this.baseUrl}/${id}`).pipe(
      map((res) => {
        const rawCats = res.categories ?? res.Categories;
        const categories = Array.isArray(rawCats)
          ? rawCats.map((x) => String(x))
          : [];
        const rawIds = res.categoryIds ?? res.CategoryIds;
        const categoryIds = Array.isArray(rawIds)
          ? rawIds.map((x) => Number(x))
          : [];
        const genreJoined = categories.length ? categories.join(', ') : null;
        return {
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
          genre: (res.genre ?? res.Genre ?? genreJoined) as string | null,
          categories: categories.length ? categories : null,
          categoryIds,
          releaseDate:
            (res.releaseDate ??
              res.ReleaseDate ??
              (res.releaseYear ?? res.ReleaseYear)?.toString()) as string | null,
          durationMinutes: Number(
            res.durationInMinutes ?? res.DurationInMinutes ?? 0
          ),
          releaseYear: Number(res.releaseYear ?? res.ReleaseYear ?? 0),
          imdbRating: parseImdbRating(res.imdbRating ?? res.ImdbRating),
          imdbId:
            ((res as { imdbId?: string; ImdbId?: string }).imdbId ??
              (res as { ImdbId?: string }).ImdbId ??
              null) as string | null
        };
      })
    );
  }

  getShowtimesByMovie(movieId: string | number): Observable<DateShowtimes[]> {
    return this.http
      .get<GetShowtimesByMovieResponse>(
        `${API_BASE_URL}/ShowTimes/by-movie/${movieId}`
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

  /** Admin: liste — id, title, kategoriler, posterUrl (imageUrl) */
  getMoviesForAdmin(): Observable<AdminMovieRow[]> {
    return this.http.get<GetAllMoviesResponse>(this.baseUrl).pipe(
      map((res) => {
        const movies = Array.isArray(res?.movies) ? res.movies : [];
        return movies.map((m) => {
          const cats = m.categories ?? m.Categories;
          const label = Array.isArray(cats) ? cats.join(', ') : '';
          return {
            id: Number(m.id ?? m.Id ?? 0),
            title: String(m.title ?? m.Title ?? ''),
            categoriesLabel: label,
            posterUrl: ((m.imageUrl ?? m.ImageUrl ?? null) as string | null) ?? null
          };
        });
      })
    );
  }

  createMovie(payload: CreateMoviePayload): Observable<MovieMutationResult> {
    return this.http
      .post<Record<string, unknown>>(this.baseUrl, payload)
      .pipe(
        map((res) => ({
          succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
          message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null,
          id: Number(res?.['id'] ?? res?.['Id'] ?? 0) || undefined,
          imdbRating: (res?.['imdbRating'] ?? res?.['ImdbRating'] ?? null) as
            | number
            | null
            | undefined
        })),
        catchError((err: HttpErrorResponse) => of(this.mapMutationError(err)))
      );
  }

  updateMovie(payload: UpdateMoviePayload): Observable<MovieMutationResult> {
    const body = {
      ...payload,
      isCompleted: false
    };
    return this.http.put<Record<string, unknown>>(this.baseUrl, body).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null
      })),
      catchError((err: HttpErrorResponse) => of(this.mapMutationError(err)))
    );
  }

  /** Admin: afiş dosyası yükler; dönen URL `imageUrl` alanına yazılır. */
  uploadMoviePoster(file: File): Observable<{ url: string }> {
    const fd = new FormData();
    fd.append('file', file);
    return this.http
      .post<Record<string, unknown>>(`${this.baseUrl}/upload-poster`, fd)
      .pipe(
        map((res) => {
          const u = (res?.['url'] ?? res?.['Url']) as string | undefined;
          if (!u) {
            throw new Error('Sunucudan URL alınamadı.');
          }
          return { url: u };
        }),
        catchError((err: HttpErrorResponse) => {
          const body = err.error as { message?: string; Message?: string } | null;
          const msg =
            body && (body.message ?? body.Message)
              ? String(body.message ?? body.Message)
              : err.status === 403 || err.status === 401
                ? 'Afiş yüklemek için Admin olarak giriş yapın.'
                : 'Afiş yüklenemedi.';
          return throwError(() => new Error(msg));
        })
      );
  }

  /** Admin: tüm filmlerin IMDb puanını OMDb ile günceller. */
  syncImdbRatings(): Observable<SyncImdbRatingsResult> {
    return this.http
      .post<Record<string, unknown>>(`${this.baseUrl}/sync-imdb-ratings`, {})
      .pipe(
        map((res) => ({
          succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
          message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null,
          totalMovies: Number(res?.['totalMovies'] ?? res?.['TotalMovies'] ?? 0),
          updatedCount: Number(res?.['updatedCount'] ?? res?.['UpdatedCount'] ?? 0),
          skippedNoRatingCount: Number(
            res?.['skippedNoRatingCount'] ?? res?.['SkippedNoRatingCount'] ?? 0
          ),
          failedCount: Number(res?.['failedCount'] ?? res?.['FailedCount'] ?? 0)
        })),
        catchError((err: HttpErrorResponse) =>
          of({
            succeeded: false,
            message: this.mapMutationError(err).message,
            totalMovies: 0,
            updatedCount: 0,
            skippedNoRatingCount: 0,
            failedCount: 0
          })
        )
      );
  }

  deleteMovie(id: number): Observable<MovieMutationResult> {
    return this.http.delete<Record<string, unknown>>(`${this.baseUrl}/${id}`).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null
      })),
      catchError((err: HttpErrorResponse) => of(this.mapMutationError(err)))
    );
  }

  private mapMutationError(err: HttpErrorResponse): MovieMutationResult {
    const body = err.error as { message?: string; Message?: string } | null;
    const msg =
      body && (body.message ?? body.Message)
        ? String(body.message ?? body.Message)
        : err.status === 403 || err.status === 401
          ? 'Bu işlem için Admin yetkisi gerekir.'
          : 'İşlem başarısız oldu.';
    return { succeeded: false, message: msg };
  }
}

