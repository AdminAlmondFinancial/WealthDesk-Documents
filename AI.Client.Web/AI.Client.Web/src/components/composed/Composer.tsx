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

export default function Composer({ inputRef, value, style, attachment, attachKey, busy, canSend, onChange, onStyle, onFile, onRemove, onSend }: Props) {
  const onKey = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      if (!busy) onSend()
    }
  }
  const handleFile = (e: ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0]
    if (f) onFile(f)
    e.target.value = ''
  }
  return (
    <div className="dock after">
      <div className="cwrap">
        <div className="composer">
          {attachment && (
            <div className="chip" id="chip" key={attachKey}>
              <Sheet size="sm" n={5} />
              <div><b id="chipName">{attachment.name}</b><small id="chipMeta">{attachment.meta}</small></div>
              <button className="icon-btn" type="button" aria-label="Remove attachment" onClick={onRemove} style={{ width: 28, height: 28, borderRadius: 8 }}><CloseIcon /></button>
              <span className="bar" aria-hidden="true"></span>
            </div>
          )}
          <label htmlFor="input" className="sr">Message the document assistant</label>
          <textarea
            id="input"
            ref={inputRef}
            rows={2}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            onKeyDown={onKey}
            placeholder={attachment ? 'Add any instructions, or press Enter to format' : 'Describe how the document should look, or attach a file'}
          />
          <div className="crow">
            <label className="attach" title="Attach a document">
              <input type="file" id="file" accept=".doc,.docx,.pdf,.txt,.rtf,.odt" aria-label="Attach a document" onChange={handleFile} />
              <ClipIcon />
            </label>
            <div className="style-pick">
              <label htmlFor="style" className="sr">House style</label>
              <select id="style" value={style} onChange={(e) => onStyle(e.target.value)}>
                {HOUSE_STYLES.map((s) => <option key={s}>{s}</option>)}
              </select>
              <SelectChevronIcon />
            </div>
            <div className="spacer"></div>
            <span className="hint">Shift + Enter for a new line</span>
            <button className="send" type="button" id="send" aria-label={busy ? 'Stop formatting' : 'Send'} title={busy ? 'Stop formatting' : 'Send'} disabled={!canSend} onClick={onSend}>
              <SendIcon />
              <StopIcon />
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
