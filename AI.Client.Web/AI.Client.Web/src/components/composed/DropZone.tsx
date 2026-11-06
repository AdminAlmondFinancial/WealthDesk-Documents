import Sheet from './Sheet'

export default function DropZone({ hidden }: { hidden: boolean }) {
  return (
    <div className="drop" id="drop" hidden={hidden}>
      <Sheet size="md" n={5} extra="loop" />
      <strong>Drop to attach</strong>
      <span>Word, PDF or text files</span>
    </div>
  )
}
