import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router'
import AuthProviderModule from 'react-auth-kit/AuthProvider'
import createStoreModule from 'react-auth-kit/createStore'
import App from './App'
import { DESIGN_MODE } from './lib/designMode'
import './index.css'

// react-auth-kit ships CommonJS, so its default export can arrive wrapped in { default }.
const unwrap = <T,>(m: T): T => ((m as unknown as { default?: T }).default ?? m)
const AuthProvider = unwrap(AuthProviderModule)
const createStore = unwrap(createStoreModule)
const store = createStore({ authName: '_auth', authType: 'localstorage' })

async function start() {
  if (DESIGN_MODE) {
    const { worker } = await import('./mocks/browser')
    await worker.start({ onUnhandledFrame: 'bypass' })
  }
  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <AuthProvider store={store}>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </AuthProvider>
    </StrictMode>,
  )
}

start()
