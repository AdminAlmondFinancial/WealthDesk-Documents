import type { ChangeEvent, KeyboardEvent, RefObject } from 'react'
import Sheet from './Sheet'
import { ClipIcon, CloseIcon, SelectChevronIcon, SendIcon, StopIcon } from './Icons'
import { HOUSE_STYLES, type Attachment } from '@/pages/Assistant/assistantTypes'

interface Props {
  inputRef: RefObject<HTMLTextAreaElement | null>
  value: string
  style: string
  attachment: Attachment | null
  attachKey: number
  busy: boolean
  canSend: boolean
  onChange: (v: string) => void
  onStyle: (v: string) => void
  onFile: (f: File) => void
  onRemove: () => void
  onSend: () => void
}

export default function Composer(p: Props) {
  const onKey = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      if (!p.busy) p.onSend()
    }
  }
  const onFile = (e: ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0]
    if (f) p.onFile(f)
    e.target.value = ''
  }
  return (
    <div className="dock after">
      <div className="cwrap">
        <div className="composer">
          {p.attachment && (
            <div className="chip" id="chip" key={p.attachKey}>
              <Sheet size="sm" n={5} />
              <div><b id="chipName">{p.attachment.name}</b><small id="chipMeta">{p.attachment.meta}</small></div>
              <button className="icon-btn" type="button" aria-label="Remove attachment" onClick={p.onRemove} style={{ width: 28, height: 28, borderRadius: 8 }}><CloseIcon /></button>
              <span className="bar" aria-hidden="true"></span>
            </div>
          )}
          <label htmlFor="input" className="sr">Message the document assistant</label>
          <textarea
            id="input"
            ref={p.inputRef}
            rows={2}
            value={p.value}
            onChange={(e) => p.onChange(e.target.value)}
            onKeyDown={onKey}
            placeholder={p.attachment ? 'Add any instructions, or press Enter to format' : 'Describe how the document should look, or attach a file'}
          />
          <div className="crow">
            <label className="attach" title="Attach a document">
              <input type="file" id="file" accept=".doc,.docx,.pdf,.txt,.rtf,.odt" aria-label="Attach a document" onChange={onFile} />
              <ClipIcon />
            </label>
            <div className="style-pick">
              <label htmlFor="style" className="sr">House style</label>
              <select id="style" value={p.style} onChange={(e) => p.onStyle(e.target.value)}>
                {HOUSE_STYLES.map((s) => <option key={s}>{s}</option>)}
              </select>
              <SelectChevronIcon />
            </div>
            <div className="spacer"></div>
            <span className="hint">Shift + Enter for a new line</span>
            <button className="send" type="button" id="send" aria-label={p.busy ? 'Stop formatting' : 'Send'} title={p.busy ? 'Stop formatting' : 'Send'} disabled={!p.canSend} onClick={p.onSend}>
              <SendIcon />
              <StopIcon />
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
