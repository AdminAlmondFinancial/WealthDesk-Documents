import Sheet from './Sheet'
import { ListIcon } from './Icons'

interface Props {
  greeting: string
  suggestions: [string, string][]
  onPick: (text: string, style: string) => void
}

export function Hero({ greeting }: { greeting: string }) {
  return (
    <>
      <div className="grid" aria-hidden="true"></div>
      <section className="hero">
        <Sheet size="lg" n={6} extra="settle" />
        <div className="hero-copy">
          <h2 className="serif" id="greeting">{greeting}</h2>
          <p>Attach a document and choose a house style. You get back a clean copy with consistent headings, numbering, tables and fonts.</p>
        </div>
      </section>
    </>
  )
}

export function Suggestions({ suggestions, onPick }: Pick<Props, 'suggestions' | 'onPick'>) {
  return (
    <div className="suggest after" id="suggest">
      {suggestions.map(([text, style]) => (
        <button key={text} type="button" onClick={() => onPick(text, style)}>
          <ListIcon />
          <span>{text}</span>
          <small>{style}</small>
        </button>
      ))}
    </div>
  )
}
