import { PlusIcon } from '@/components/composed/Icons'
import type { CurrentUser, HistoryGroup } from '@/pages/Assistant/assistantTypes'

interface Props {
  history: HistoryGroup[]
  activeId: string | null
  user: CurrentUser | null
  onNewChat: () => void
  onSelect: (id: string, title: string) => void
}

const initials = (n: string) => n.split(/\s+/).filter(Boolean).map((p) => p[0]).slice(0, 2).join('').toUpperCase()

export default function Sidebar({ history, activeId, user, onNewChat, onSelect }: Props) {
  return (
    <aside className="side" aria-label="Chats">
      <div className="brand">
        <svg width="32" height="32" viewBox="0 0 30 30" role="img" aria-label="Wealth Desk"><rect width="30" height="30" rx="9" fill="#6D28D9" /><path d="M7.5 10l3.5 11 4-8 4 8 3.5-11" fill="none" stroke="#FFFFFF" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" /></svg>
        <div><b>WEALTH DESK</b><span>Document assistant</span></div>
      </div>
      <button className="btn-secondary" type="button" id="newChat" onClick={onNewChat}>
        <PlusIcon />
        New chat
      </button>
      <nav className="history" aria-label="Recent chats" id="history">
        {history.map((g) => (
          <div className="group" key={g.label}>
            <h2>{g.label}</h2>
            {g.items.map(([id, title]) => (
              <button key={id} className="nav" type="button" aria-current={activeId === id ? 'true' : 'false'} onClick={() => onSelect(id, title)}>{title}</button>
            ))}
          </div>
        ))}
      </nav>
      {user && (
        <div className="me">
          <div className="avatar" aria-hidden="true">{initials(user.name)}</div>
          <div><b>{user.name}</b><span>{user.role}</span></div>
        </div>
      )}
    </aside>
  )
}
