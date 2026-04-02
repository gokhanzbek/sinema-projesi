import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { catchError, distinctUntilChanged, map, Observable, of } from 'rxjs';
import { API_BASE_URL } from '../core/api-base';
import {
  clearAllTokenStorage,
  readBearerTokenForHttp,
  readStoredAccessToken,
  TOKEN_EXPIRATION_KEY,
  TOKEN_KEY
} from '../core/auth-storage';
import { hasAdminRole } from '../core/jwt-claims';
import { initialsFromAccessToken } from '../core/jwt-initials';

export interface RegisterPayload {
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly loginUrl = `${API_BASE_URL}/Users/Login`;
  private readonly usersUrl = `${API_BASE_URL}/Users`;

  private readonly loggedIn = new BehaviorSubject<boolean>(this.computeLoggedIn());
  readonly isLoggedIn$ = this.loggedIn.asObservable();

  /** Giriş durumu değişince token’dan yeniden hesaplanır */
  readonly isAdmin$ = this.loggedIn.pipe(
    map(() => this.isAdmin()),
    distinctUntilChanged()
  );

  constructor(private readonly http: HttpClient) {}

  private computeLoggedIn(): boolean {
    return !!readStoredAccessToken();
  }

  /** Navbar vb. için: token’dan baş harfler */
  getUserInitials(): string {
    const token = readBearerTokenForHttp() ?? readStoredAccessToken();
    return initialsFromAccessToken(token);
  }

  getAccessToken(): string | null {
    const t = readStoredAccessToken();
    if (t) return t;
    if (readBearerTokenForHttp()) {
      this.clearAuth();
    }
    return null;
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }

  /** JWT’de Admin rolü var mı */
  isAdmin(): boolean {
    return hasAdminRole(this.getAccessToken());
  }

  setAuth(accessToken: string, expiration: string | Date): void {
    clearAllTokenStorage();
    localStorage.setItem(TOKEN_KEY, accessToken);
    const exp =
      typeof expiration === 'string' ? expiration : expiration.toISOString();
    localStorage.setItem(TOKEN_EXPIRATION_KEY, exp);
    this.loggedIn.next(true);
  }

  clearAuth(): void {
    clearAllTokenStorage();
    this.loggedIn.next(false);
  }

  /** Çıkış: token temizlenir; çağıran Router ile yönlendirir */
  logout(): void {
    this.clearAuth();
  }

  login(usernameOrEmail: string, password: string): Observable<{ ok: boolean; message?: string }> {
    return this.http
      .post<Record<string, unknown>>(this.loginUrl, {
        usernameOrEmail,
        password
      })
      .pipe(
        map((body) => {
          const token = body?.['token'] ?? body?.['Token'];
          if (!token || typeof token !== 'object') {
            return { ok: false, message: 'Beklenmeyen yanıt.' };
          }
          const t = token as Record<string, unknown>;
          const access =
            (t['accessToken'] ?? t['AccessToken'] ?? '') as string;
          const exp = (t['expiration'] ?? t['Expiration'] ?? '') as string;
          if (!access) {
            return { ok: false, message: 'Token alınamadı.' };
          }
          this.setAuth(access, exp || new Date(Date.now() + 3600_000).toISOString());
          return { ok: true };
        }),
        catchError((err) => {
          const msg =
            err?.error?.message ??
            err?.error?.title ??
            'Giriş başarısız. Bilgilerinizi kontrol edin.';
          return of({ ok: false, message: String(msg) });
        })
      );
  }

  register(payload: RegisterPayload): Observable<{ succeeded: boolean; message: string }> {
    return this.http
      .post<{
        succeeded?: boolean;
        Succeeded?: boolean;
        message?: string;
        Message?: string;
      }>(this.usersUrl, payload)
      .pipe(
        map((res) => ({
          succeeded: !!(res?.succeeded ?? res?.Succeeded),
          message: String(res?.message ?? res?.Message ?? '')
        })),
        catchError((err) => {
          const msg =
            err?.error?.message ??
            err?.error?.title ??
            'Kayıt sırasında hata oluştu.';
          return of({ succeeded: false, message: String(msg) });
        })
      );
  }
}
