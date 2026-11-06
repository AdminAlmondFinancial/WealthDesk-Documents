export default function Sheet({ size, n, extra = '' }: { size: 'sm' | 'md' | 'lg'; n: number; extra?: string }) {
  return (
    <div className={`sheet ${size}${extra ? ' ' + extra : ''}`} aria-hidden="true">
      {Array.from({ length: n }, (_, i) => (
        <i key={i} />
      ))}
    </div>
  )
}
