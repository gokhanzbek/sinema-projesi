import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';

export interface ShowtimeDetailDto {
  id: number;
  startTime: string;
  price: number;
  movieId: number;
  hallId: number;
  hallRowCount: number;
  hallColumnCount: number;
}

export interface CreateTicketPayload {
  showtimeId: number;
  seatNumbers: string[];
  price: number;
}

export interface CreateTicketResult {
  succeeded: boolean;
  message: string | null;
}

/** API my-tickets satırı (camelCase / PascalCase normalize edilir) */
export interface MyTicketDto {
  id: number;
  seatNumber: string;
  price: number;
  showtimeId: number;
  movieTitle: string;
  showtimeStart: string;
  status: number;
}

export interface GetMyTicketsResult {
  succeeded: boolean;
  message: string | null;
  tickets: MyTicketDto[];
}

export interface CancelTicketResult {
  succeeded: boolean;
  message: string | null;
}

@Injectable({ providedIn: 'root' })
export class TicketService {
  private readonly ticketsUrl = `${API_BASE_URL}/Tickets`;
  private readonly showtimesUrl = `${API_BASE_URL}/ShowTimes`;

  constructor(private readonly http: HttpClient) {}

  getShowtimeById(showtimeId: number): Observable<ShowtimeDetailDto> {
    return this.http
      .get<Record<string, unknown>>(`${this.showtimesUrl}/${showtimeId}`)
      .pipe(
        map((r) => {
          if (r == null || typeof r !== 'object') {
            return {
              id: 0,
              startTime: '',
              price: 0,
              movieId: 0,
              hallId: 0,
              hallRowCount: 0,
              hallColumnCount: 0
            };
          }
          return {
            id: Number(r['id'] ?? r['Id'] ?? 0),
            startTime: String(r['startTime'] ?? r['StartTime'] ?? ''),
            price: Number(r['price'] ?? r['Price'] ?? 0),
            movieId: Number(r['movieId'] ?? r['MovieId'] ?? 0),
            hallId: Number(r['hallId'] ?? r['HallId'] ?? 0),
            hallRowCount: Number(r['hallRowCount'] ?? r['HallRowCount'] ?? 0),
            hallColumnCount: Number(r['hallColumnCount'] ?? r['HallColumnCount'] ?? 0)
          };
        })
      );
  }

  getOccupiedSeats(showtimeId: number): Observable<string[]> {
    return this.http
      .get<{ occupiedSeats?: string[]; OccupiedSeats?: string[] }>(
        `${this.ticketsUrl}/occupied-seats/${showtimeId}`
      )
      .pipe(
        map((res) => {
          const list = res?.occupiedSeats ?? res?.OccupiedSeats ?? [];
          return Array.isArray(list) ? list : [];
        })
      );
  }

  getMyTickets(): Observable<GetMyTicketsResult> {
    return this.http
      .get<Record<string, unknown>>(`${this.ticketsUrl}/my-tickets`)
      .pipe(
        map((res) => {
          const succeeded = !!(res?.['succeeded'] ?? res?.['Succeeded']);
          const message = (res?.['message'] ?? res?.['Message'] ?? null) as string | null;
          const raw = (res?.['tickets'] ?? res?.['Tickets'] ?? []) as unknown[];
          const tickets: MyTicketDto[] = Array.isArray(raw)
            ? raw.map((row) => this.safeNormalizeMyTicket(row))
            : [];
          return { succeeded, message, tickets };
        })
      );
  }

  private safeNormalizeMyTicket(row: unknown): MyTicketDto {
    try {
      return this.normalizeMyTicket(row);
    } catch {
      return {
        id: 0,
        seatNumber: '',
        price: 0,
        showtimeId: 0,
        movieTitle: '',
        showtimeStart: '',
        status: 1
      };
    }
  }

  private normalizeMyTicket(row: unknown): MyTicketDto {
    const r = row as Record<string, unknown>;
    const statusRaw = r['status'] ?? r['Status'];
    let status = typeof statusRaw === 'number' ? statusRaw : parseInt(String(statusRaw), 10);
    if (Number.isNaN(status)) status = 1;

    return {
      id: Number(r['id'] ?? r['Id'] ?? 0),
      seatNumber: String(r['seatNumber'] ?? r['SeatNumber'] ?? ''),
      price: Number(r['price'] ?? r['Price'] ?? 0),
      showtimeId: Number(r['showtimeId'] ?? r['ShowtimeId'] ?? 0),
      movieTitle: String(r['movieTitle'] ?? r['MovieTitle'] ?? r['title'] ?? r['Title'] ?? ''),
      showtimeStart: String(r['showtimeStart'] ?? r['ShowtimeStart'] ?? ''),
      status
    };
  }

  createTicket(payload: CreateTicketPayload): Observable<CreateTicketResult> {
    return this.http
      .post<{
        succeeded?: boolean;
        Succeeded?: boolean;
        message?: string;
        Message?: string;
      }>(this.ticketsUrl, payload)
      .pipe(
        map((res) => ({
          succeeded: !!(res?.succeeded ?? res?.Succeeded),
          message: (res?.message ?? res?.Message ?? null) as string | null
        }))
      );
  }

  /** POST api/Tickets/cancel/{id} — bilet durumunu iptal eder (sunucu doğrular). */
  cancelTicket(ticketId: number): Observable<CancelTicketResult> {
    return this.http
      .post<{
        succeeded?: boolean;
        Succeeded?: boolean;
        message?: string;
        Message?: string;
      }>(`${this.ticketsUrl}/cancel/${ticketId}`, {})
      .pipe(
        map((res) => ({
          succeeded: !!(res?.succeeded ?? res?.Succeeded),
          message: (res?.message ?? res?.Message ?? null) as string | null
        })),
        catchError((err: HttpErrorResponse) => {
          const body = err.error as { message?: string; Message?: string } | null;
          const msg =
            body && (body.message ?? body.Message)
              ? String(body.message ?? body.Message)
              : err.status === 0
                ? 'Ağ hatası veya sunucuya ulaşılamadı.'
                : 'İptal işlemi başarısız oldu.';
          return of({ succeeded: false, message: msg });
        })
      );
  }
}
