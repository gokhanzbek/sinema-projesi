import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';

export interface AdminHallRow {
  id: number;
  name: string;
  capacity: number;
  rowCount: number;
  columnCount: number;
}

export interface HallMutationResult {
  succeeded: boolean;
  message: string | null;
  id?: number;
}

export interface HallPayload {
  name: string;
  rowCount: number;
  columnCount: number;
}

export interface UpdateHallPayload extends HallPayload {
  id: number;
}

interface GetAllHallsApiResponse {
  totalCount?: number;
  TotalCount?: number;
  halls?: Array<{
    id?: number;
    Id?: number;
    name?: string;
    Name?: string;
    capacity?: number;
    Capacity?: number;
    rowCount?: number;
    RowCount?: number;
    columnCount?: number;
    ColumnCount?: number;
  }>;
  Halls?: Array<{
    id?: number;
    Id?: number;
    name?: string;
    Name?: string;
    capacity?: number;
    Capacity?: number;
    rowCount?: number;
    RowCount?: number;
    columnCount?: number;
    ColumnCount?: number;
  }>;
}

@Injectable({ providedIn: 'root' })
export class HallService {
  private readonly url = `${API_BASE_URL}/Halls`;

  constructor(private readonly http: HttpClient) {}

  getAllHalls(): Observable<AdminHallRow[]> {
    return this.http.get<GetAllHallsApiResponse>(this.url).pipe(
      map((res) => {
        const list = res?.halls ?? res?.Halls ?? [];
        return Array.isArray(list)
          ? list.map((h) => ({
              id: Number(h.id ?? h.Id ?? 0),
              name: String(h.name ?? h.Name ?? ''),
              capacity: Number(h.capacity ?? h.Capacity ?? 0),
              rowCount: Number(h.rowCount ?? h.RowCount ?? 0),
              columnCount: Number(h.columnCount ?? h.ColumnCount ?? 0)
            }))
          : [];
      })
    );
  }

  createHall(payload: HallPayload): Observable<HallMutationResult> {
    return this.http.post<Record<string, unknown>>(this.url, payload).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null,
        id: Number(res?.['id'] ?? res?.['Id'] ?? 0) || undefined
      })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  updateHall(payload: UpdateHallPayload): Observable<HallMutationResult> {
    return this.http.put<Record<string, unknown>>(this.url, payload).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess']),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null
      })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  deleteHall(id: number): Observable<HallMutationResult> {
    return this.http.delete<Record<string, unknown>>(`${this.url}/${id}`).pipe(
      map((res) => ({
        succeeded: !!(res?.['isSuccess'] ?? res?.['IsSuccess'] ?? true),
        message: (res?.['message'] ?? res?.['Message'] ?? null) as string | null
      })),
      catchError((err: HttpErrorResponse) => of(this.mapErr(err)))
    );
  }

  private mapErr(err: HttpErrorResponse): HallMutationResult {
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
