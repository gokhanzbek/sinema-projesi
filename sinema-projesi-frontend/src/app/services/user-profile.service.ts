import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';

export interface UserProfile {
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
}

@Injectable({ providedIn: 'root' })
export class UserProfileService {
  private readonly baseUrl = `${API_BASE_URL}/Users`;

  constructor(private readonly http: HttpClient) {}

  getProfile(): Observable<UserProfile> {
    return this.http.get<Record<string, unknown>>(`${this.baseUrl}/me`).pipe(
      map((res) => ({
        firstName: String(res['firstName'] ?? res['FirstName'] ?? ''),
        lastName: String(res['lastName'] ?? res['LastName'] ?? ''),
        email: String(res['email'] ?? res['Email'] ?? ''),
        userName: String(res['userName'] ?? res['UserName'] ?? '')
      }))
    );
  }

  changePassword(payload: {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
  }): Observable<{ succeeded: boolean; message: string }> {
    return this.http
      .post<Record<string, unknown>>(`${this.baseUrl}/change-password`, {
        currentPassword: payload.currentPassword,
        newPassword: payload.newPassword,
        confirmNewPassword: payload.confirmNewPassword
      })
      .pipe(
        map((res) => ({
          succeeded: !!(res['succeeded'] ?? res['Succeeded']),
          message: String(res['message'] ?? res['Message'] ?? '')
        })),
        catchError((err: HttpErrorResponse) => {
          const raw = err.error;
          const body =
            raw && typeof raw === 'object'
              ? (raw as Record<string, unknown>)
              : null;
          const fromApi = body
            ? String(body['message'] ?? body['Message'] ?? '')
            : '';
          const msg =
            fromApi ||
            (err.status === 401
              ? 'Oturum süresi doldu. Tekrar giriş yapın.'
              : 'Şifre güncellenemedi.');
          return throwError(() => new Error(msg));
        })
      );
  }
}
