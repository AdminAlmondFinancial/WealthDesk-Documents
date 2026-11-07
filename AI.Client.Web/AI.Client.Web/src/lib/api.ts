import { timezoneHeader } from './timezoneHeader'
import { DESIGN_MODE } from './designMode'
import { getAccessToken } from './auth'

export const API_BASE: string = import.meta.env.VITE_API_BASE ?? '/api'

export async function api<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  for (const [k, v] of Object.entries(timezoneHeader())) headers.set(k, v)
  if (!DESIGN_MODE) {
    const token = await getAccessToken()
    if (token) headers.set('Authorization', `Bearer ${token}`)
  }
  const res = await fetch(API_BASE + path, { ...init, headers })
  if (!res.ok) throw new Error(`Request failed (${res.status})`)
  return res.json() as Promise<T>
}

/** Downloads a file from the API (with the bearer token) and saves it under the given name. */
export async function downloadFile(path: string, filename: string): Promise<void> {
  const headers = new Headers()
  if (!DESIGN_MODE) {
    const token = await getAccessToken()
    if (token) headers.set('Authorization', `Bearer ${token}`)
  }
  const res = await fetch(API_BASE + path, { headers })
  if (!res.ok) throw new Error(`Request failed (${res.status})`)
  const url = URL.createObjectURL(await res.blob())
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  a.click()
  URL.revokeObjectURL(url)
}
