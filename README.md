# CommentMap

A pet-project web app where users pin comments to locations on an interactive map. Each comment's coordinates are resolved to a country via a spatial query against PostGIS.

## Features

- Place comments on an interactive map (OpenLayers)
- Automatic country detection from coordinates via a PostGIS spatial query (NetTopologySuite, SRID 3857)
- Full identity management with ASP.NET Core Identity: registration, email confirmation, password reset, two-factor authentication, profile management
- External login via Google OAuth
- Asynchronous email delivery: messages are published to RabbitMQ and rendered/sent by a background EmailSender service using MJML templates
- Local orchestration of all dependencies (PostGIS, RabbitMQ, MailPit) with .NET Aspire

## Tech stack

- **Backend:** .NET 10, ASP.NET Core Razor Pages, Wolverine (message bus / mediator), EF Core + Npgsql with NetTopologySuite
- **Infrastructure:** .NET Aspire AppHost, PostGIS, RabbitMQ, MailPit
- **Frontend:** TypeScript, esbuild, Tailwind CSS v4, daisyUI, OpenLayers
- **Email:** RazorBlade templates compiled to MJML (Mjml.Net), sent via MailKit SMTP

## Prerequisites

- .NET 10 SDK
- Node.js
- Docker (Aspire runs the PostGIS, RabbitMQ, and MailPit containers)

## Getting started

1. Restore pinned local tools:

   ```sh
   dotnet tool restore
   ```

2. Run the Aspire AppHost:

   ```sh
   dotnet run --project src/CommentMap.AppHost
   ```

   This starts PostGIS (+ pgAdmin), RabbitMQ, and MailPit containers, applies EF Core migrations via the MigrationService, and then starts the MVC app and the EmailSender.

3. Open the Aspire dashboard from the URL printed in the console and navigate to the `mvc` resource endpoint.

Notes:

- The frontend is built automatically during `dotnet build` (the `BuildJS` target runs `npm install` + `npm run build`) — no separate frontend step is needed.
- Database migrations are applied automatically at startup by the MigrationService; do not run `dotnet ef database update` manually.
- Emails are caught by MailPit in development — check its UI (linked from the Aspire dashboard) for confirmation emails, since confirmed accounts are required.

### Optional: Google OAuth

To enable Google sign-in, set user secrets for the MVC project:

```sh
dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>" --project src/CommentMap.Mvc
dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>" --project src/CommentMap.Mvc
```

## Project structure

| Project | Description |
| --- | --- |
| `CommentMap.AppHost` | .NET Aspire orchestrator — wires up PostGIS, RabbitMQ, MailPit, and all services |
| `CommentMap.Mvc` | Razor Pages web app (UI, identity, message publishing) |
| `CommentMap.Application` | Feature handlers — one file per message (`record` + static handler class) |
| `CommentMap.Infrastructure` | EF Core `CommentMapDbContext`, PostGIS mappings, migrations |
| `CommentMap.MigrationService` | Applies EF Core migrations at startup |
| `CommentMap.EmailSender` | Background service that consumes email messages from RabbitMQ, renders MJML templates, and sends via SMTP |
| `CommentMap.Shared` | Shared message contracts |
| `CommentMap.ServiceDefaults` | Aspire service defaults (telemetry, health checks, resilience) |

## License

[MIT](LICENSE.txt)
