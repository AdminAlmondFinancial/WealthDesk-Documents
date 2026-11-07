import { useEffect, useState } from 'react'
import { Outlet } from 'react-router'
import { DESIGN_MODE } from '@/lib/designMode'
import { initAuth, signIn } from '@/lib/auth'
import SignIn from '@/pages/SignIn/SignIn'

type State = 'loading' | 'signedIn' | 'signedOut'

export default function ProtectedRoute() {
  const [state, setState] = useState<State>(DESIGN_MODE ? 'signedIn' : 'loading')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (DESIGN_MODE) return
    initAuth()
      .then((account) => setState(account ? 'signedIn' : 'signedOut'))
      .catch(() => {
        setError('Sign-in failed. Try again.')
        setState('signedOut')
      })
  }, [])

  if (state === 'loading') return null
  if (state === 'signedIn') return <Outlet />
  return <SignIn onSignIn={() => signIn().catch(() => setError('Sign-in failed. Try again.'))} error={error} />
}
