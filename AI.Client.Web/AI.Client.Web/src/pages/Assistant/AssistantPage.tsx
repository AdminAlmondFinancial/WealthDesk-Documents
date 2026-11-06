import { useCallback, useEffect, useRef, useState } from 'react'
import AppShell from '@/templates/AppShell/AppShell'
import Composer from '@/components/composed/Composer'
import DropZone from '@/components/composed/DropZone'
import MessageItem from '@/components/composed/MessageItem'
import WorkingPanel from '@/components/composed/WorkingPanel'
import { Hero, Suggestions } from '@/components/composed/StartScreen'
import { api } from '@/lib/api'
import {
  HOUSE_STYLES, MODEL_NAME, SUGGESTIONS, WORK_MS, fmtSize, greeting, kind, stepsFor,
  type AssistantMessage, type Attachment, type ChatResult, type CurrentUser, type FormatResult,
  type HistoryGroup, type Message,
} from './assistantTypes'

interface Work { steps: string[] | null; progress: number; start: number }

export default function AssistantPage() {
  const [user, setUser] = useState<CurrentUser | null>(null)
  const [history, setHistory] = useState<HistoryGroup[]>([])
  const [messages, setMessages] = useState<Message[]>([])
  const [text, setText] = useState('')
  const [style, setStyle] = useState(HOUSE_STYLES[0])
  const [attachment, setAttachment] = useState<Attachment | null>(null)
  const [attachKey, setAttachKey] = useState(0)
  const [work, setWork] = useState<Work | null>(null)
  const [dragging, setDragging] = useState(false)

  const fileRef = useRef<File | null>(null)
  const inputRef = useRef<HTMLTextAreaElement | null>(null)
  const logRef = useRef<HTMLDivElement | null>(null)
  const seq = useRef(1)
  const dragDepth = useRef(0)
  const timers = useRef<ReturnType<typeof setTimeout>[]>([])
  const abort = useRef<AbortController | null>(null)
  const run = useRef(0)

  const busy = work !== null
  const hasMessages = messages.length > 0 || busy

  useEffect(() => {
    api<CurrentUser>('/me').then(setUser).catch(() => {})
    api<HistoryGroup[]>('/chats').then(setHistory).catch(() => {})
  }, [])

  useEffect(() => { if (logRef.current) logRef.current.scrollTop = logRef.current.scrollHeight }, [messages, work])

  const clearTimers = () => { timers.current.forEach(clearTimeout); timers.current = [] }
  const first = messages.find((m) => m.role === 'user')
  const title = !first || first.role !== 'user' ? 'New chat'
    : first.file ? first.file.name.replace(/\.[^.]+$/, '')
    : first.text.length > 48 ? first.text.slice(0, 48) + '…' : first.text

  const attach = (f: File | null) => {
    fileRef.current = f
    setAttachment(f ? { name: f.name, meta: kind(f.name) + ', ' + fmtSize(f.size) } : null)
    setAttachKey((k) => k + 1)
  }

  const push = (m: Message) => setMessages((ms) => [...ms, m])

  const stop = () => {
    run.current++
    clearTimers()
    abort.current?.abort()
    setWork(null)
    push({ id: 'a' + ++seq.current, role: 'assistant', text: 'Stopped. The document was not changed.' })
  }

  const send = () => {
    if (busy) { stop(); return }
    const msg = text.trim(), att = attachment, file = fileRef.current, t = style
    if (!msg && !att) return
    const id = (seq.current += 2), start = Date.now(), myRun = ++run.current
    push({ id: 'u' + id, role: 'user', text: msg, file: att })
    setText(''); attach(null)
    const steps = att ? stepsFor(t) : null
    setWork({ steps, progress: 0, start })
    const ac = (abort.current = new AbortController())
    const wait = (ms: number) => new Promise<void>((r) => timers.current.push(setTimeout(r, ms)))

    if (steps && file) {
      const per = WORK_MS / steps.length
      steps.forEach((_, i) => timers.current.push(setTimeout(() => setWork((w) => (w ? { ...w, progress: i + 1 } : w)), per * (i + 1))))
      const body = new FormData()
      body.append('file', file); body.append('style', t); body.append('instructions', msg)
      Promise.all([api<FormatResult>('/documents/format', { method: 'POST', body, signal: ac.signal }), wait(WORK_MS + 250)])
        .then(([res]) => {
          if (run.current !== myRun) return
          clearTimers(); setWork(null)
          const reply: AssistantMessage = {
            id: 'a' + id, role: 'assistant', took: Math.round((Date.now() - start) / 100) / 10, steps,
            text: 'Done. ' + att!.name + ' now follows the ' + t + '. Here is what changed:', changes: res.changes, out: res.out,
          }
          push(reply)
        })
        .catch(() => { if (run.current === myRun) { clearTimers(); setWork(null); push({ id: 'a' + id, role: 'assistant', text: 'Something went wrong. The document was not changed.' }) } })
    } else {
      Promise.all([
        api<ChatResult>('/chat', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ text: msg, style: t, model: MODEL_NAME }), signal: ac.signal }),
        wait(1100),
      ])
        .then(([res]) => { if (run.current === myRun) { clearTimers(); setWork(null); push({ id: 'a' + id, role: 'assistant', text: res.text }) } })
        .catch(() => { if (run.current === myRun) { clearTimers(); setWork(null); push({ id: 'a' + id, role: 'assistant', text: 'Something went wrong. Please try again.' }) } })
    }
  }

  const newChat = useCallback(() => {
    run.current++; clearTimers(); abort.current?.abort(); setWork(null)
    setMessages([]); setActiveId(null); setText(''); attach(null)
    inputRef.current?.focus()
  }, [])
  const [activeId, setActiveId] = useState<string | null>(null)

  const hasFiles = (e: React.DragEvent) => Array.prototype.indexOf.call(e.dataTransfer?.types || [], 'Files') >= 0
  const mainHandlers = {
    onDragEnter: (e: React.DragEvent) => { if (!hasFiles(e)) return; e.preventDefault(); dragDepth.current++; setDragging(true) },
    onDragOver: (e: React.DragEvent) => { if (hasFiles(e)) e.preventDefault() },
    onDragLeave: () => { dragDepth.current = Math.max(0, dragDepth.current - 1); if (!dragDepth.current) setDragging(false) },
    onDrop: (e: React.DragEvent) => { e.preventDefault(); dragDepth.current = 0; setDragging(false); if (e.dataTransfer.files[0]) attach(e.dataTransfer.files[0]) },
  }

  const firstName = user ? user.name.split(/\s+/)[0] : ''
  const hello = greeting() + (firstName ? ', ' + firstName : '')

  return (
    <AppShell
      hasMessages={hasMessages}
      busy={busy}
      title={title}
      history={history}
      activeId={activeId}
      user={user}
      onNewChat={newChat}
      onSelect={(id) => setActiveId(id)}
      mainHandlers={mainHandlers}
      overlay={<DropZone hidden={!dragging} />}
    >
      <div className="body">
        <Hero greeting={hello} />
        <div className="log" id="log" role="log" aria-live="polite" aria-label="Conversation" aria-busy={busy} hidden={!hasMessages} ref={logRef}>
          <div className="thread" id="thread">
            {messages.map((m) => <MessageItem key={m.id} m={m} fresh />)}
            {work && <WorkingPanel steps={work.steps} progress={work.progress} start={work.start} />}
          </div>
        </div>
        <Composer
          inputRef={inputRef}
          value={text}
          style={style}
          attachment={attachment}
          attachKey={attachKey}
          busy={busy}
          canSend={busy || !!text.trim() || !!attachment}
          onChange={setText}
          onStyle={setStyle}
          onFile={attach}
          onRemove={() => { attach(null); inputRef.current?.focus() }}
          onSend={send}
        />
        <Suggestions suggestions={SUGGESTIONS} onPick={(t, s) => { setText(t); setStyle(s); inputRef.current?.focus() }} />
        <p className="disclaimer">{MODEL_NAME} can make mistakes. Check formatted documents before they go to clients.</p>
      </div>
    </AppShell>
  )
}
