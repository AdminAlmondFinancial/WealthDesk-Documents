import { useState } from 'react'
import Sheet from './Sheet'
import { CheckIcon, ChevronIcon, CircleCheckIcon, CopyIcon, DownloadIcon, EyeIcon, LogoMark, RedoIcon, TickIcon } from './Icons'
import { WORD_MS, type Message } from '@/pages/Assistant/assistantTypes'

export default function MessageItem({ m, fresh }: { m: Message; fresh: boolean }) {
  const [open, setOpen] = useState(false)
  const [copied, setCopied] = useState(false)
  const [notice, setNotice] = useState(false)

  if (m.role === 'user') {
    return (
      <div className={'u' + (fresh ? ' in' : '')}>
        {m.file && (
          <div className="file">
            <Sheet size="sm" n={5} />
            <div><b>{m.file.name}</b><small>{m.file.meta}</small></div>
          </div>
        )}
        {m.text && <div className="bubble">{m.text}</div>}
      </div>
    )
  }

  const words = m.text.split(/\s+/).filter(Boolean)
  const t0 = words.length * WORD_MS + 200
  const nChanges = m.changes ? m.changes.length : 0

  const copy = () => {
    const done = () => {
      setCopied(true)
      setTimeout(() => setCopied(false), 1600)
    }
    try { navigator.clipboard.writeText(m.text).then(done, done) } catch { done() }
  }

  return (
    <div className="a">
      <LogoMark />
      <div className="a-body">
        {m.steps && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            <button className="took" type="button" aria-expanded={open} onClick={() => setOpen(!open)}>
              <CircleCheckIcon />Formatted in {m.took} seconds<ChevronIcon className="chev" />
            </button>
            <ul className="steps" hidden={!open}>
              {m.steps.map((s) => (
                <li key={s}><span className="slot"><CheckIcon /></span>{s}</li>
              ))}
            </ul>
          </div>
        )}
        <p>
          {words.map((w, i) => (
            <span key={i} className={fresh ? 'w' : undefined} style={fresh ? { animationDelay: i * WORD_MS + 'ms' } : undefined}>{w + ' '}</span>
          ))}
        </p>
        {m.changes && (
          <ul className="changes">
            {m.changes.map((c, i) => (
              <li key={c} className={fresh ? 'in' : undefined} style={fresh ? { animationDelay: t0 + i * 110 + 'ms' } : undefined}>
                <span className="tick" aria-hidden="true"><TickIcon /></span><span>{c}</span>
              </li>
            ))}
          </ul>
        )}
        {m.out && (
          <>
            <div className={'out' + (fresh ? ' reveal' : '')} style={fresh ? { animationDelay: t0 + nChanges * 110 + 120 + 'ms' } : undefined}>
              <Sheet size="md" n={6} />
              <div className="meta"><b>{m.out.name}</b><small>{m.out.meta}</small></div>
              <div className="acts">
                <button className="btn sec" type="button" onClick={() => setNotice(true)}><EyeIcon />Preview</button>
                <button className="btn outl" type="button" onClick={() => setNotice(true)}><DownloadIcon />Download</button>
              </div>
            </div>
            <p className="note" hidden={!notice}>This is a design preview. Files are produced once the formatting service is connected.</p>
          </>
        )}
        <div className="tools">
          <button className="icon-btn sm" type="button" aria-label={copied ? 'Copied' : 'Copy reply'} title="Copy reply" onClick={copy}>
            {copied ? (<><span className="pop" style={{ display: 'inline-flex' }}><CheckIcon /></span><span>Copied</span></>) : <CopyIcon />}
          </button>
          <button className="icon-btn" type="button" aria-label="Format again" title="Format again" style={{ width: 32, height: 32 }}><RedoIcon /></button>
        </div>
      </div>
    </div>
  )
}
