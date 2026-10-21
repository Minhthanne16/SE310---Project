import type { ApiResponse } from '@/types/api';

export async function apiRequest<T>(
  path: string,
  init?: RequestInit,
): Promise<T> {
  const response = await fetch(`/api${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...init?.headers },
    cache: 'no-store',
  });
  let body: ApiResponse<T>;
  try {
    body = (await response.json()) as ApiResponse<T>;
  } catch {
    throw new Error(`Backend unavailable (HTTP ${response.status})`);
  }
  if (!response.ok || !body.success) {
    throw new Error(body.message || `HTTP ${response.status}`);
  }
  return body.data;
}
