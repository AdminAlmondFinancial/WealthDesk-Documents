import { Outlet } from 'react-router'

// Cloudflare Access signs people in before they reach the site, so the app has no login of its own.
export default function ProtectedRoute() {
  return <Outlet />
}
