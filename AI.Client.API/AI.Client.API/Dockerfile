# Build context is the AI.Client.API root (the folder holding AI.Client.slnx).
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY nuget.config AI.Client.slnx ./
COPY AI.Client.API/AI.Client.API.csproj AI.Client.API/
RUN dotnet restore AI.Client.API/AI.Client.API.csproj
COPY AI.Client.API/ AI.Client.API/
RUN dotnet publish AI.Client.API/AI.Client.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Configuration keys (set as environment variables at deploy time; the double underscore is the ':' separator).
# Secrets (marked *) come from Key Vault or pipeline secrets and are never baked into the image.
#   ConnectionStrings__Default        Postgres connection string *
#   Access__TeamDomain                Cloudflare Access team domain
#   Access__Audience                  AUD tag of the Access application for this backend
#   Access__ServiceTokenClientId      Client id of the Worker service token
#   Orchestrator__BaseUrl             wealthdesk-documents Worker URL
#   Orchestrator__AccessClientId      Cloudflare Access service token id *
#   Orchestrator__AccessClientSecret  Cloudflare Access service token secret *
#   Cors__AllowedOrigins__0           Frontend origin
ENTRYPOINT ["dotnet", "AI.Client.API.dll"]
