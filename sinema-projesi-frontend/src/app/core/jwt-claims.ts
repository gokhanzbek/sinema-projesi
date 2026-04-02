/** JWT payload parse + rol çıkarımı (ASP.NET Identity claim isimleri) */

const ROLE_URI = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

export function parseJwtPayload(token: string | null | undefined): Record<string, unknown> | null {
  if (!token?.trim()) return null;
  try {
    const part = token.split('.')[1];
    if (!part) return null;
    const base64 = part.replace(/-/g, '+').replace(/_/g, '/');
    const pad = base64.length % 4 === 0 ? '' : '='.repeat(4 - (base64.length % 4));
    const json = atob(base64 + pad);
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

function pushRole(out: string[], v: unknown): void {
  if (v == null) return;
  if (Array.isArray(v)) {
    v.forEach((x) => pushRole(out, x));
    return;
  }
  const s = String(v).trim();
  if (s) out.push(s);
}

/** Token’daki tüm rol claim değerlerini döner (string dizisi). */
export function getRolesFromToken(token: string | null | undefined): string[] {
  const p = parseJwtPayload(token);
  if (!p) return [];
  const roles: string[] = [];
  pushRole(roles, p[ROLE_URI]);
  pushRole(roles, p['role']);
  pushRole(roles, p['Role']);
  pushRole(roles, p['roles']);
  return roles;
}

export function hasAdminRole(token: string | null | undefined): boolean {
  return getRolesFromToken(token).some((r) => r.toLowerCase() === 'admin');
}
