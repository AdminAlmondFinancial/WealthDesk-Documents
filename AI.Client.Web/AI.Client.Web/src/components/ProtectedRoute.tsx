import { Outlet } from 'react-router'
import useIsAuthenticatedModule from 'react-auth-kit/hooks/useIsAuthenticated'
import { DESIGN_MODE } from '@/lib/designMode'

// react-auth-kit ships CommonJS, so its default export can arrive wrapped in { default }.
const useIsAuthenticated = (useIsAuthenticatedModule as unknown as { default?: typeof useIsAuthenticatedModule }).default ?? useIsAuthenticatedModule

export default function ProtectedRoute() {
  const isAuthenticated = useIsAuthenticated()
  // Sign-in is out of scope for this UI; until it exists, an unauthenticated visitor sees nothing.
  return DESIGN_MODE || isAuthenticated ? <Outlet /> : null
}
