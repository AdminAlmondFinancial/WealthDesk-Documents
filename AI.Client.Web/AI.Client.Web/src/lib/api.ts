import { timezoneHeader } from './timezoneHeader'

export const API_BASE: string = import.meta.env.VITE_API_BASE ?? '/api'

export async function api<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  for (const [k, v] of Object.entries(timezoneHeader())) headers.set(k, v)
  const res = await fetch(API_BASE + path, { ...init, headers })
  if (!res.ok) throw new Error(`Request failed (${res.status})`)
  return res.json() as Promise<T>
}
