import { useEffect, useState, type DragEventHandler, type ReactNode } from 'react'
import Sidebar from './Sidebar'
import TopBar from './TopBar'
import type { CurrentUser, HistoryGroup } from '@/pages/Assistant/assistantTypes'

interface Props {
  hasMessages: boolean
  busy: boolean
  title: string
  history: HistoryGroup[]
  activeId: string | null
  user: CurrentUser | null
  onNewChat: () => void
  onSelect: (id: string, title: string) => void
  mainHandlers: {
    onDragEnter: DragEventHandler
    onDragOver: DragEventHandler
    onDragLeave: DragEventHandler
    onDrop: DragEventHandler
  }
  overlay: ReactNode
  children: ReactNode
}

export default function AppShell(p: Props) {
  const [navOpen, setNavOpen] = useState(false)
  useEffect(() => {
    const h = (e: KeyboardEvent) => { if (e.key === 'Escape') setNavOpen(false) }
    document.addEventListener('keydown', h)
    return () => document.removeEventListener('keydown', h)
  }, [])
  const cls = ['app', p.hasMessages && 'has-msgs', p.busy && 'busy', navOpen && 'nav-open'].filter(Boolean).join(' ')

  return (
    <div className={cls} id="app">
      <button className="scrim" type="button" aria-label="Close chats" id="scrim" onClick={() => setNavOpen(false)}></button>
      <Sidebar
        history={p.history}
        activeId={p.activeId}
        user={p.user}
        onNewChat={() => { p.onNewChat(); setNavOpen(false) }}
        onSelect={(id, t) => { p.onSelect(id, t); setNavOpen(false) }}
      />
      <main className="main" id="main" {...p.mainHandlers}>
        {p.overlay}
        <TopBar title={p.title} busy={p.busy} onMenu={() => setNavOpen(true)} />
        {p.children}
      </main>
    </div>
  )
}
