# WealthDesk Documents

Document AI microservice for Wealth Desk. The repo has two main folders: this one (`AI.Client.Web`, the frontend) and `../AI.Client.API` (the C# backend, see its own README).

## Layout (this folder)
- `AI.Client.Web/` — Vite + React 19 + TypeScript frontend (the Document Assistant UI)
- `start-dev.bat` — Windows dev launcher
- `Deployment/pipelines/` — Azure DevOps pipeline for the frontend
- `memory/` — project notes for Claude Code
- `CLAUDE.md`, `DESIGN.md`, `PRODUCT.md` — project rules, design source of truth, product notes

## Run the frontend
```
cd AI.Client.Web/AI.Client.Web   # from the repo root
npm install
npm run dev
```
On Windows, run `AI.Client.Web/start-dev.bat` from the repo root. With `VITE_DESIGN_MODE=true` (set in `.env.local`) the UI runs against MSW mock data, so no backend is needed.
