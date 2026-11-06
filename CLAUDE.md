# CLAUDE.md

Frontend for the Wealth Desk Document Assistant. Pure frontend: no .NET code in this repo yet.

## Stack
Vite + React 19 + TypeScript, React Router v7, Tailwind v4 (theme and utilities only, no preflight), MSW for mock data, react-auth-kit v3.

## Rules
- The design must match `DESIGN.md` exactly. Do not restyle.
- Each page is a feature folder under `src/pages/<Page>/` with `<Page>.tsx` and `<page>Types.ts`.
- Mock data lives in `src/mocks/` and only runs when `VITE_DESIGN_MODE=true`.
- `src/stores/` and `src/hooks/` are empty placeholders.
- Prefer the edit tool over `sed -i`. Do not add files outside the existing structure.

## Commands
Run inside `AI.Client.Web/`: `npm run dev`, `npm run build`, `npm run lint`.
