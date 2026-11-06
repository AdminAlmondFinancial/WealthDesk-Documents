# Design

Source of truth: the "Wealth Desk Document Assistant" artifact. The CSS in `AI.Client.Web/src/index.css` is a verbatim copy; change both together.

## Fonts
- Display: Newsreader (400, 500) for the greeting, output file name, drop zone
- UI: Schibsted Grotesk (400-700)

## Colour tokens (light / dark)
| Token | Light | Dark |
|---|---|---|
| `--bg` | #FAF9FD | #100D17 |
| `--surface` | #FFFFFF | #17131F |
| `--ink` | #1E1B2E | #F2EFF9 |
| `--muted` | #6B6680 | #ABA5BC |
| `--border` | #E7E4F0 | #2A2538 |
| `--p700` (primary) | #6D28D9 | #7C3AED |
| `--p-soft` / `--p-tint` / `--p-line` | #EDE7FD / #F7F4FE / #DDD2FB | #2C2245 / #1E1830 / #40336A |
| `--success` | #15803D | #4ADE80 |

Dark mode follows `prefers-color-scheme`, or `data-theme="dark"` / `"light"` on `:root`.

## Layout
- App shell: 256px sidebar on the canvas, conversation on a white rounded sheet (16px radius), 14px gap.
- Under 768px the sidebar becomes a slide-in drawer with a scrim.
- Thread and composer max width 760px.

## Signature elements
- The "sheet" motif: a small page whose lines tidy themselves (sm / md / lg sizes, `settle` and `loop` animations).
- Composer: 22px radius, purple glow on focus, rotating conic-gradient border while busy.
- Assistant replies: word-by-word blur-in, staggered change list, output card with a light sweep.
- Model pill shows the Microsoft logo (replaces the status dot) and pulses while working.
- All motion is disabled under `prefers-reduced-motion`.
