import { DotsIcon, MenuIcon } from '@/components/composed/Icons'
import { MODEL_NAME } from '@/pages/Assistant/assistantTypes'

interface Props {
  title: string
  busy: boolean
  onMenu: () => void
}

export default function TopBar({ title, busy, onMenu }: Props) {
  return (
    <header className="top">
      <button className="icon-btn menu-btn" type="button" aria-label="Open chats" id="menuBtn" onClick={onMenu}><MenuIcon /></button>
      <h1 id="chatTitle">{title}</h1>
      <span className="model" id="model" title={busy ? `${MODEL_NAME} is working` : MODEL_NAME}><i aria-hidden="true"></i>{MODEL_NAME}</span>
      <button className="icon-btn" type="button" aria-label="Chat options" title="Chat options"><DotsIcon /></button>
    </header>
  )
}
