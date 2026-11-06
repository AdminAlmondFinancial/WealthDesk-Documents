export function hasPermission(granted: string[], required: string): boolean {
  return granted.includes(required)
}
