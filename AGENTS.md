# CommentMap — Agent Guide

## Run

- Prereqs: .NET 10 SDK, Node.js, Docker (Aspire runs containers).
- `dotnet tool restore` — restores pinned local tools (`dotnet-ef`, `aspire`, `roslyn-language-server`).
- Local tools are NOT global: invoke them via `dotnet` — `dotnet aspire start|stop|ps|logs|doctor`, `dotnet-ef ...`. A bare `aspire` command does not exist in PATH (and `aspire start` fails with `CommandNotFoundException`) — always `dotnet aspire ...`.
- `dotnet aspire start` — starts PostGIS + RabbitMQ + MailPit containers and all services (`dotnet aspire stop` to shut down). The MVC app waits for MigrationService to apply EF migrations before starting.
- Dev params for RabbitMQ/Postgres live in the AppHost **user secrets** (`dotnet user-secrets -p src/CommentMap.AppHost set "Parameters:pg-password" ...`), not in appsettings. Google OAuth keys come from Mvc user secrets (`Authentication:Google:ClientId` / `ClientSecret`).

## Build & Frontend

- `dotnet build` automatically runs `npm install` + `npm run build` in `CommentMap.Mvc/` (a `BuildJS` target in the csproj) — no separate frontend step.
- Frontend entrypoints are hardcoded in `CommentMap.Mvc/build/build.js`. Adding a new `.ts` page script requires registering it there, or it will silently not be built.
- Stack: esbuild + Tailwind v4 (via PostCSS) + daisyUI + OpenLayers (`ol`). Output goes to `CommentMap.Mvc/wwwroot/{js,css}`. All projects live under `src/`.

## Architecture

- Aspire AppHost (`CommentMap.AppHost/AppHost.cs`) orchestrates: `postgis/postgis:18-3.6` (+ pgAdmin), RabbitMQ 4.3.4 (management plugin), MailPit. The EF connection string name is `"comment-map"` (Aspire resource name, not a config-section path).
- Razor Pages page models never touch EF directly. They call `IMessageBus.InvokeAsync(...)` (Wolverine) with a message record; handlers are static classes in `CommentMap.Application/Features/**` — one file per message, `record Xxx` + `static class XxxHandler` with a `Handle` method. Follow this pattern for new features.
- DB access goes through `ICommentMapDbContext` (`CommentMap.Application/Abstractions`), implemented by `CommentMapDbContext` in Infrastructure. Spatial data uses NetTopologySuite `Point` with SRID 3857 against PostGIS.
- Email flow: Mvc publishes `CommentMap.Shared/Messages/*` records to RabbitMQ queues named `nameof(Message)`; EmailSender listens on the same names, renders a RazorBlade `.cshtml` template to MJML (Mjml.Net), and sends via MailKit SMTP (MailPit in dev — check its UI for confirmation emails, since `RequireConfirmedAccount = true`). SMTP settings bind from the `Aspire:Smtp` config section (`SmtpSettings`); TLS is configured via `Security` (MailKit `SecureSocketOptions`, defaults to `None` for MailPit).
- Health endpoints: Mvc maps `/health` and `/alive` in dev via `MapDefaultEndpoints()` (ServiceDefaults). EmailSender is a headless worker — no HTTP endpoints.

## Wolverine codegen (important)

- Mvc and EmailSender both set `TypeLoadMode.Static`. The generated handlers under `*/Internal/Generated/WolverineHandlers/` are **committed to git** — do not hand-edit or delete them.
- After adding or changing a message handler, regenerate:
  `dotnet run --project src/CommentMap.Mvc -- codegen write` (same for `CommentMap.EmailSender`).
- Wolverine handler discovery includes the `CommentMap.Application` assembly (see Mvc `Program.cs`) — handlers added there are picked up automatically.

## EF Core / Migrations

- Add a migration: `dotnet ef migrations add <Name> --project src/CommentMap.Infrastructure --startup-project src/CommentMap.Mvc`
- Do NOT run `dotnet ef database update` locally — MigrationService applies migrations automatically at startup.

## Conventions & Gotchas

- No test projects, no lint/format config — verification = build + run via AppHost. CI: `.github/workflows/publish-container.yml` builds & pushes the Mvc container image to GHCR on push to `master` (no Dockerfile — uses `dotnet publish /t:PublishContainer`; npm/esbuild run on the runner via the `BuildJS` csproj target).
- `opencode.json` configures the C# LSP via the local `roslyn-language-server` tool; run `dotnet tool restore` first or the LSP won't start.
- Solution file is `CommentMap.slnx` (XML format), not `.sln`.
