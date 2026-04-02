import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';

export interface AdminSessionRow {
  id: number;
  movieName: string;
  hallName: string;
  date: string;
  time: string;
}

export interface AdminSessionDetail {
  id: number;
  startTime: string;
  price: number;
  movieId: number;
  hallId: number;
}

export interface SessionMutationResult {
  succeeded: boolean;
  message: string | null;
}

export interface SessionPayload {
  movieId: number;
  hallId: number;
  price: number;
  /** ISO 8601 */
  startTime: string;
}

export interface UpdateSessionPayload extends SessionPayload {
  id: number;
}

interface GetAllShowTimesApiResponse {
  totalCount?: number;
  TotalCount?: number;
  showTimes?: unknown[];
  ShowTimes?: unknown[];
}

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly url = `${API_BASE_URL}/ShowTimes`;

  constructor(private readonly http: HttpClient) {}

  getAllSessions(): Observable<AdminSessionRow[]> {
    return this.http.get<GetAllShowTimesApiResponse>(this.url).pipe(
      map((res) => {
        const raw = res?.showTimes ?? res?.ShowTimes ?? [];
        if (!Array.isArray(raw)) return [];
        return raw.map((row) => this.normalizeRow(row));
      })
    );
  }

  getSessionById(id: number): Observable<AdminSessionDetail | null> {
    return this.http.get<Record<string, unknown>>(`${this.url}/${id}`).pipe(
      map((res) => {
        if (res == null || typeof res !== 'object') return null;
        const idVal = Number(res['id'] ?? res['Id'] ?? 0);
        if (!idVal) return null;
        const st = res['startTime'] ?? res['StartTime'];
        const startIso =
          st instanceof Date
            ? st.toISOString()
            : typeof st === 'string'
              ? st
              : new Date(String(st)).toISOString();
        return {
          id: idVal,
          startTime: startIso,
          price: Number(res['price'] ?? res['Price'] ?? 0),
          movieId: Number(res['movieId'] ?? res['MovieId'] ?? 0),
          hallId: Number(res['hallId'] ?? res['HallId'] ?? 0)
        };
      }),
      catchError(() => of(null))
    );
  }

  createSession(payload: SessionPayload): Observable<SessionMutationResult> {
    return this.http.post<Record<string, unknown>>(this.url, payload).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null
      })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  updateSession(payload: UpdateSessionPayload): Observable<SessionMutationResult> {
    return this.http.put<Record<string, unknown>>(this.url, payload).pipe(
      map(() => ({ succeeded: true, message: null as string | null })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  deleteSession(id: number): Observable<SessionMutationResult> {
    return this.http.delete(`${this.url}/${id}`).pipe(
      map(() => ({ succeeded: true, message: null as string | null })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  private mapErr(err: HttpErrorResponse): SessionMutationResult {
    const body = err.error as { message?: string; Message?: string } | null;
    const msg =
      body && (body.message ?? body.Message)
        ? String(body.message ?? body.Message)
        : err.status === 403 || err.status === 401
          ? 'Bu işlem için Admin yetkisi gerekir.'
          : 'İşlem başarısız oldu.';
    return { succeeded: false, message: msg };
  }

  private normalizeRow(row: unknown): AdminSessionRow {
    const r = row as Record<string, unknown>;
    const id = Number(r['id'] ?? r['Id'] ?? 0);
    const movieName = String(r['movieName'] ?? r['MovieName'] ?? '');
    const hallName = String(r['hallName'] ?? r['HallName'] ?? '');
    const startRaw = r['startTime'] ?? r['StartTime'];
    const { date, time } = this.splitDateTime(startRaw);
    return { id, movieName, hallName, date, time };
  }

  private splitDateTime(value: unknown): { date: string; time: string } {
    if (value == null) return { date: '—', time: '—' };
    const d = new Date(String(value));
    if (Number.isNaN(d.getTime())) return { date: '—', time: '—' };
    return {
      date: new Intl.DateTimeFormat('tr-TR', { dateStyle: 'medium' }).format(d),
      time: new Intl.DateTimeFormat('tr-TR', { timeStyle: 'short' }).format(d)
    };
  }
}
