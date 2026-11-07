import { PublicClientApplication, type AccountInfo } from '@azure/msal-browser'

const clientId: string = import.meta.env.VITE_ENTRA_CLIENT_ID ?? ''
const tenantId: string = import.meta.env.VITE_ENTRA_TENANT_ID ?? ''

export const loginScopes = [`api://${clientId}/access_as_user`]

export const msal = new PublicClientApplication({
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri: window.location.origin,
  },
  cache: { cacheLocation: 'sessionStorage' },
})

let ready: Promise<AccountInfo | null> | null = null

/** Initialises MSAL once and restores the signed-in account, if any. */
export function initAuth(): Promise<AccountInfo | null> {
  ready ??= msal.initialize().then(async () => {
    const result = await msal.handleRedirectPromise()
    const account = result?.account ?? msal.getAllAccounts()[0] ?? null
    if (account) msal.setActiveAccount(account)
    return account
  })
  return ready
}

/** Starts the "Continue with Microsoft" redirect. */
export async function signIn(): Promise<void> {
  await initAuth()
  await msal.loginRedirect({ scopes: loginScopes })
}

/** Returns a bearer token for the API, or null when nobody is signed in. */
export async function getAccessToken(): Promise<string | null> {
  const account = await initAuth()
  if (!account) return null
  const result = await msal.acquireTokenSilent({ scopes: loginScopes, account })
  return result.accessToken
}
