# WealthDesk Documents

Document AI microservice for Wealth Desk. This branch holds the frontend only; the C# backend is added later.

## Layout
- `AI.Client.Web/AI.Client.Web/` — Vite + React 19 + TypeScript frontend (the Document Assistant UI); `AI.Client.Web/` also holds `start-dev.bat`
- `Deployment/pipelines/` — Azure DevOps pipelines
- `memory/` — project notes for Claude Code

## Run the frontend
```
cd AI.Client.Web/AI.Client.Web
npm install
npm run dev
```
On Windows, run `AI.Client.Web/start-dev.bat`. With `VITE_DESIGN_MODE=true` (set in `.env.local`) the UI runs against MSW mock data, so no backend is needed.
