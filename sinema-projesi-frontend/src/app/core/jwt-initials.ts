/** JWT payload'dan görünen ad / kullanıcı adına göre baş harfler (avatar). */
export function initialsFromAccessToken(token: string | null | undefined): string {
  if (!token?.trim()) return '?';
  try {
    const part = token.split('.')[1];
    if (!part) return 'U';
    const base64 = part.replace(/-/g, '+').replace(/_/g, '/');
    const pad = base64.length % 4 === 0 ? '' : '='.repeat(4 - (base64.length % 4));
    const json = atob(base64 + pad);
    const p = JSON.parse(json) as Record<string, unknown>;

    const claimName =
      p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];

    const name =
      (typeof claimName === 'string' && claimName) ||
      (typeof p['unique_name'] === 'string' && p['unique_name']) ||
      (typeof p['name'] === 'string' && p['name']) ||
      (typeof p['preferred_username'] === 'string' && p['preferred_username']) ||
      (typeof p['email'] === 'string' && p['email']) ||
      '';

    if (name.includes('@')) {
      const local = name.split('@')[0] ?? '';
      if (local.length >= 2) return local.slice(0, 2).toUpperCase();
      return (local.slice(0, 1) || '?').toUpperCase();
    }

    const words = name.trim().split(/\s+/).filter(Boolean);
    if (words.length >= 2) {
      return (words[0][0] + words[1][0]).toUpperCase();
    }
    if (words.length === 1 && words[0].length >= 2) {
      return words[0].slice(0, 2).toUpperCase();
    }
    return (words[0]?.[0] ?? 'U').toUpperCase();
  } catch {
    return 'U';
  }
}
