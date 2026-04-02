/**
 * Token anahtarları — AuthService ve interceptor ortak kullanır.
 */

export const TOKEN_KEY = 'token';
export const TOKEN_EXPIRATION_KEY = 'token_expiration';
export const LEGACY_TOKEN_KEY = 'cinema_access_token';
export const LEGACY_EXPIRATION_KEY = 'cinema_token_expiration';

const ALL_TOKEN_KEYS = [
  TOKEN_KEY,
  'accessToken',
  'access_token',
  LEGACY_TOKEN_KEY
] as const;

function stripBearerPrefix(raw: string): string {
  const t = raw.trim();
  if (t.toLowerCase().startsWith('bearer ')) {
    return t.slice(7).trim();
  }
  return t;
}

/** localStorage'dan ilk bulunan JWT (birçok olası anahtar). */
function getRawTokenFromStorage(): string | null {
  for (const k of ALL_TOKEN_KEYS) {
    const v = localStorage.getItem(k)?.trim();
    if (v) return stripBearerPrefix(v);
  }
  return null;
}

/**
 * Oturum / isLoggedIn için: süresi dolmuşsa null (temizlik AuthService'de).
 */
export function readStoredAccessToken(): string | null {
  const token = getRawTokenFromStorage();
  if (!token) return null;

  const expRaw =
    localStorage.getItem(TOKEN_EXPIRATION_KEY) ??
    localStorage.getItem(LEGACY_EXPIRATION_KEY);
  if (expRaw) {
    const exp = new Date(expRaw).getTime();
    if (!Number.isNaN(exp) && exp <= Date.now()) {
      return null;
    }
  }
  return token;
}

/**
 * HTTP Authorization için: süreyi istemcide kesmeyiz; sunucu 401 döner.
 * Böylece tarih/saat farkı veya yanlış parse yüzünden Bearer eklenmemesi önlenir.
 */
export function readBearerTokenForHttp(): string | null {
  return getRawTokenFromStorage();
}

/** Tüm olası token / süre anahtarlarını temizler. */
export function clearAllTokenStorage(): void {
  const keys = [
    ...ALL_TOKEN_KEYS,
    TOKEN_EXPIRATION_KEY,
    LEGACY_EXPIRATION_KEY
  ];
  for (const k of keys) {
    localStorage.removeItem(k);
  }
}
