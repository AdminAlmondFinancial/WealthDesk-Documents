import { useEffect, useState } from 'react'
import Sheet from './Sheet'
import { CheckIcon, LogoMark } from './Icons'

interface Props {
  steps: string[] | null
  progress: number
  start: number
}

export default function WorkingPanel({ steps, progress, start }: Props) {
  const [now, setNow] = useState(() => Date.now())
  useEffect(() => {
    const t = setInterval(() => setNow(Date.now()), 100)
    return () => clearInterval(t)
  }, [])
  const label = steps ? steps[Math.min(progress, steps.length - 1)] : 'Thinking'

  return (
    <div className="a in">
      <LogoMark />
      <div className="a-body" style={{ gap: 12, paddingTop: 2 }}>
        <div className="working-head">
          <Sheet size="sm" n={5} extra="loop" />
          <span className="shimmer">{label}</span>
          <span className="elapsed">{((now - start) / 1000).toFixed(1)}s</span>
        </div>
        {steps && (
          <ul className="steps">
            {steps.map((s, i) => {
              const st = i < progress ? 'done' : i === progress ? 'active' : 'pending'
              return (
                <li key={s} className={st}>
                  <span className="slot">
                    {st === 'done' ? <span className="pop" style={{ display: 'inline-flex' }}><CheckIcon /></span> : st === 'active' ? <span className="spin" aria-hidden="true"></span> : <span className="ring"></span>}
                  </span>
                  {s}
                </li>
              )
            })}
          </ul>
        )}
      </div>
    </div>
  )
}
