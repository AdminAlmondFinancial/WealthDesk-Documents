import { http, HttpResponse } from 'msw'
import { API_BASE } from '@/lib/api'

// Mock data for DESIGN_MODE only. Mirrors the shapes in pages/Assistant/assistantTypes.ts.
export const handlers = [
  http.get(`${API_BASE}/me`, () => HttpResponse.json({ name: 'Sarah Thompson', role: 'Paraplanner' })),

  http.get(`${API_BASE}/chats`, () => HttpResponse.json([])),

  http.post(`${API_BASE}/documents/format`, async ({ request }) => {
    const form = await request.formData()
    const file = form.get('file') as File
    const style = String(form.get('style') ?? '')
    return HttpResponse.json({
      changes: [
        'Applied the ' + style + ' heading styles and body font',
        'Renumbered sections and rebuilt the contents page',
        'Aligned tables, with money columns right-aligned in £',
        'Set dates to dd/mm/yyyy and evened out paragraph spacing',
      ],
      out: {
        name: file.name.replace(/\.[^.]+$/, '') + ' (formatted).docx',
        meta: 'Word document, formatted to the ' + style,
      },
    })
  }),

  http.post(`${API_BASE}/chat`, async ({ request }) => {
    const { style } = (await request.json()) as { style: string }
    return HttpResponse.json({
      text:
        'Attach a Word or PDF document with the paperclip, or drop it anywhere on this page, and I will format it to the ' +
        style +
        '.',
    })
  }),
]
