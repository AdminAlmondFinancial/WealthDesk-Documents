# AI.Client.API

C# backend for the Wealth Desk Document Assistant. It serves the React frontend in `../AI.Client.Web` and calls the document AI through the `wealthdesk-documents` Cloudflare Worker.

## Layout
See `CLAUDE.md` for the structure rules. Projects:

| Project | Purpose |
|---|---|
| `AI.Client.API` | ASP.NET Core Web API |
| `AI.Client.DataMigration` | DbUp migrator (embedded `scripts/Script{NNNN} - *.sql`) |
| `AI.Client.UnitTests` | Validation, entities, manager, orchestrator client |
| `AI.Client.IntegrationTests` | `WebApplicationFactory` with `Mock*DataStore` classes |
| `AI.Client.DatabaseIntegrationTests` | Real Postgres (creates and drops its own database) |

## Endpoints (all require a Microsoft Entra bearer token)
| Method | Path | Notes |
|---|---|---|
| POST | `/api/auth/login` | Validates the Entra token; returns name, role and all access areas |
| GET | `/api/me` | `{name, role}` |
| GET | `/api/chats` | History groups (`Today`, `Yesterday`, ...), using the `X-Timezone` header |
| POST | `/api/documents/format` | multipart `file` (.docx/.pdf, max 2 MB), `style`, `instructions`; returns `{changes, out{id,name,meta}, warnings}` |
| GET | `/api/documents/{id}/download` | The formatted .docx |
| POST | `/api/chat` | Returns 501 until the orchestrator has a chat task |

## Orchestrator
Calls go to the Worker (`POST /api/ai/run`, task `format`) with both Cloudflare Access service-token headers on every request. Document text is extracted in C# (the orchestrator takes text, not files). Limits: 2 MB, 10-minute timeout, one retry on 502 only, never on 422. `warnings` are shown for a person to check and are never applied automatically. Logs and audit entries hold timings and status codes only, never document text.

## Configuration
Keys are listed at the bottom of `AI.Client.API/Dockerfile`. Secrets (database, Access token, AutoMapper licence key) come from user-secrets locally and Key Vault in Azure, never from source control:

```bash
cd AI.Client.API
dotnet user-secrets init
dotnet user-secrets set "Orchestrator:AccessClientId" "<id>"
dotnet user-secrets set "Orchestrator:AccessClientSecret" "<secret>"
dotnet user-secrets set "ConnectionStrings:Default" "<postgres connection string>"
```

## Run
```bash
dotnet run --project AI.Client.DataMigration -- "<connection string>"
dotnet run --project AI.Client.API
dotnet test AI.Client.slnx
```
The database tests need Postgres; set `ConnectionStrings__Default` (admin connection) or use `localhost` / `postgres` / `postgres`.
