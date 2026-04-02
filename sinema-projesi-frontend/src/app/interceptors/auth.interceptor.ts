import { HttpInterceptorFn } from '@angular/common/http';
import { readBearerTokenForHttp } from '../core/auth-storage';

/**
 * JWT Bearer — HttpInterceptorFn: `next` bir fonksiyondur (`next(req)`), class interceptor'daki `next.handle` değildir.
 * AuthService kullanılmaz: HttpClient ↔ AuthService ↔ Interceptor döngüsü isteği sonsuza kadar pending bırakabilir.
 */
function isPublicApiRequest(url: string, method: string): boolean {
  const u = url.toLowerCase();
  if (u.includes('/users/login')) return true;
  if (method === 'GET' && u.includes('/showtimes/')) return true;
  if (method === 'GET' && u.includes('/occupied-seats/')) return true;
  return false;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (isPublicApiRequest(req.url, req.method)) {
    return next(req);
  }

  const token = readBearerTokenForHttp();
  if (!token) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    })
  );
};
