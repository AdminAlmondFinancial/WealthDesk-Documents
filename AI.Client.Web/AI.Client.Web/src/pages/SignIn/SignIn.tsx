import type { SignInProps } from './signInTypes'

export default function SignIn({ onSignIn, error }: SignInProps) {
  return (
    <main style={{ minHeight: '100vh', display: 'grid', placeItems: 'center', background: 'var(--bg)', color: 'var(--ink)' }}>
      <section style={{ textAlign: 'center', maxWidth: 360, padding: 24 }}>
        <h1 className="serif">Document Assistant</h1>
        <p style={{ color: 'var(--muted)' }}>Sign in with your Wealth Desk account to continue.</p>
        <button
          type="button"
          onClick={onSignIn}
          style={{ marginTop: 16, padding: '10px 18px', borderRadius: 10, border: 'none', background: 'var(--p700)', color: '#fff', cursor: 'pointer' }}
        >
          Continue with Microsoft
        </button>
        {error ? <p role="alert" style={{ color: 'var(--muted)', marginTop: 12 }}>{error}</p> : null}
      </section>
    </main>
  )
}
