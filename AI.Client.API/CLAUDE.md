# CLAUDE.md

C# backend for the Wealth Desk Document Assistant (service `AI.Client`, root folder `AI.Client.API/`).

## Structure (do not add anything outside it)
```
AI.Client.API/                    CLAUDE.md, nuget.config, AI.Client.slnx, .gitignore, README.md
  AI.Client.API/                  Program.cs, Startup.cs, Dockerfile, appsettings.json
    Assistant/                    entities + I<Entity>, DataStores/, Manager/
    Controllers/ Dto/ Automapper/ Auditing/ Authentication/
    AccountUsers/AccessAreas.cs
    Validation/ NotificationTemplates/ UserManagerFactory/
  AI.Client.DataMigration/        Program.cs (DbUp), scripts/Script0001..0006 - *.sql
  AI.Client.UnitTests/ AI.Client.IntegrationTests/ AI.Client.DatabaseIntegrationTests/
  Deployment/pipelines/           pr.yml, full.yml, deploy-steps.yml
  memory/                         PLAN, PR
```

## Rules
- Controller -> Manager (permission checks, validation) -> DataStore (raw SQL in `const` strings) -> Postgres.
- Entities validate in their constructors: one for new items, one for existing items with an id.
- Single-client service: no `account_id`.
- Every area goes in `AccessAreas.All`; every user is granted all areas on login.
- Migrations are `Script{NNNN} - <name>.sql` with both a `<None Remove>` and an `<EmbeddedResource>` entry in the csproj. Never edit a shipped script.
- Every test project uses the same package versions. Tests are named `<Condition>_<Method>_<Expected>`.
- Orchestrator: both Access headers on every call, secrets from configuration only, 2 MB limit, 10-minute timeout, retry once on 502 only (never 422), show warnings to a person, log timings and status codes only.
- Prefer the edit tool over `sed -i`.

## Commands
Run inside `AI.Client.API/`: `dotnet build AI.Client.slnx`, `dotnet test AI.Client.slnx`.
