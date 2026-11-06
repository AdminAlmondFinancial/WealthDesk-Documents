import type { CSSProperties } from 'react'

const base = { fill: 'none', strokeLinecap: 'round', strokeLinejoin: 'round', 'aria-hidden': true } as const

export const CheckIcon = () => (
  <svg width="14" height="14" viewBox="0 0 24 24" {...base} stroke="var(--success)" strokeWidth="2"><path d="M5 12.5l4.5 4.5L19 7.5" /></svg>
)
export const TickIcon = () => (
  <svg width="12" height="12" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="2.5"><path d="M5 12.5l4.5 4.5L19 7.5" /></svg>
)
export const CircleCheckIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" {...base} stroke="var(--success)" strokeWidth="1.75"><circle cx="12" cy="12" r="9" /><path d="M8.5 12.5l2.5 2.5 4.5-5" /></svg>
)
export const ChevronIcon = ({ className = '' }: { className?: string }) => (
  <svg className={className} width="14" height="14" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M6 9l6 6 6-6" /></svg>
)
export const CopyIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><rect x="9" y="9" width="11" height="11" rx="2" /><path d="M5 15V6a2 2 0 0 1 2-2h9" /></svg>
)
export const RedoIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M20 11a8 8 0 1 0-2.3 5.7" /><path d="M20 4v7h-7" /></svg>
)
export const EyeIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12z" /><circle cx="12" cy="12" r="3" /></svg>
)
export const DownloadIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M12 4v11" /><path d="M7 10l5 5 5-5" /><path d="M5 20h14" /></svg>
)
export const PlusIcon = () => (
  <svg width="18" height="18" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M12 5v14M5 12h14" /></svg>
)
export const MenuIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M4 7h16M4 12h16M4 17h16" /></svg>
)
export const DotsIcon = () => (
  <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true"><circle cx="12" cy="5" r="1.6" /><circle cx="12" cy="12" r="1.6" /><circle cx="12" cy="19" r="1.6" /></svg>
)
export const CloseIcon = () => (
  <svg width="14" height="14" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M6 6l12 12M18 6L6 18" /></svg>
)
export const ClipIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48" /></svg>
)
export const SelectChevronIcon = () => (
  <svg width="14" height="14" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5"><path d="M6 9l6 6 6-6" /></svg>
)
export const SendIcon = () => (
  <svg className="send-i" width="20" height="20" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.75"><path d="M12 19V5" /><path d="M5 12l7-7 7 7" /></svg>
)
export const StopIcon = () => (
  <svg className="stop-i" width="14" height="14" viewBox="0 0 24 24" aria-hidden="true"><rect x="5" y="5" width="14" height="14" rx="3" fill="currentColor" /></svg>
)
export const ListIcon = () => (
  <svg width="18" height="18" viewBox="0 0 24 24" {...base} stroke="currentColor" strokeWidth="1.5" style={{ flexShrink: 0 }}><path d="M4 6h16M4 10h10M4 14h16M4 18h10" /></svg>
)
export const MicrosoftLogo = () => (
  <svg className="ms-logo" width="14" height="14" viewBox="0 0 23 23" aria-hidden="true">
    <rect x="1" y="1" width="10" height="10" fill="#F25022" />
    <rect x="12" y="1" width="10" height="10" fill="#7FBA00" />
    <rect x="1" y="12" width="10" height="10" fill="#00A4EF" />
    <rect x="12" y="12" width="10" height="10" fill="#FFB900" />
  </svg>
)
export const LogoMark =({ style }: { style?: CSSProperties }) => (
  <svg width="32" height="32" viewBox="0 0 30 30" aria-hidden="true" style={style ?? { flexShrink: 0 }}>
    <rect width="30" height="30" rx="9" fill="#6D28D9" />
    <path d="M7.5 10l3.5 11 4-8 4 8 3.5-11" fill="none" stroke="#FFFFFF" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)
