export interface Attachment {
  name: string
  meta: string
}

export interface OutputFile {
  name: string
  meta: string
}

export interface UserMessage {
  id: string
  role: 'user'
  text: string
  file: Attachment | null
}

export interface AssistantMessage {
  id: string
  role: 'assistant'
  text: string
  steps?: string[]
  took?: number
  changes?: string[]
  out?: OutputFile
}

export type Message = UserMessage | AssistantMessage

export interface CurrentUser {
  name: string
  role: string
}

export interface HistoryGroup {
  label: string
  items: [id: string, title: string][]
}

export interface FormatResult {
  changes: string[]
  out: OutputFile
}

export interface ChatResult {
  text: string
}

export const HOUSE_STYLES = [
  'Wealth Desk house style',
  'Suitability report',
  'Annual review pack',
  'Client letter',
  'File note',
]

export const SUGGESTIONS: [text: string, style: string][] = [
  ['Apply our house style to a suitability report', 'Suitability report'],
  ['Fix heading numbers and the contents page in a review pack', 'Annual review pack'],
  ['Turn meeting notes into a client file note', 'File note'],
]

export const MODEL_NAME = 'Phi-4'
export const WORK_MS = 4000
export const WORD_MS = 28

export function stepsFor(t: string): string[] {
  return [
    'Reading the document',
    'Applying the ' + (t === 'Wealth Desk house style' ? t : t.toLowerCase() + ' style'),
    'Renumbering sections and the contents page',
    'Checking tables, figures and dates',
  ]
}

export function fmtSize(b: number): string {
  return b < 1048576 ? Math.max(1, Math.round(b / 1024)) + ' KB' : (b / 1048576).toFixed(1) + ' MB'
}

export function kind(n: string): string {
  const e = (n.split('.').pop() || '').toLowerCase()
  return e === 'pdf' ? 'PDF' : e === 'doc' || e === 'docx' ? 'Word document' : e === 'txt' ? 'Text file' : 'Document'
}

export function greeting(now = new Date()): string {
  const h = now.getHours()
  return h < 12 ? 'Good morning' : h < 18 ? 'Good afternoon' : 'Good evening'
}
