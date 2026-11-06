export function timezoneHeader(): Record<string, string> {
  return { 'X-Timezone': Intl.DateTimeFormat().resolvedOptions().timeZone }
}
