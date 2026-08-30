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
- Razor Pages page models never touch EF directly and never call each other's handlers in a chain. They **inject `ICommandHandler<XxxCommand[, TResult]>` / `IQueryHandler<XxxQuery, TResult>` into the constructor directly** and call `xxxHandler.Handle(new Xxx(...), ct)` — there is **no mediator** (`IMessageMediator` was removed; reintroduce one only when a real cross-cutting pipeline behavior — validation/transaction/logging — is needed). Cross-service events go through `IEventBus.PublishAsync(...)` injected the same way. Handlers are instance classes with constructor DI in `CommentMap.Application/Features/**` — one file per message: `record XxxCommand : ICommand` / `ICommand<TResult>` + `sealed class XxxCommandHandler : ICommandHandler<XxxCommand[, TResult]>`, or `record XxxQuery : IQuery<T>` + `sealed class XxxQueryHandler : IQueryHandler<XxxQuery, T>`, all with `Handle(Xxx, CancellationToken)`. Handler registration is scan-based (`AddMessageHandlers()`). Failure states cross the boundary as the `IdentityFailure` enum (never string error-code matching).
- Single-database CQRS-lite is intentional: commands mutate via EF/`SaveChangesAsync`, queries return DTO projections with `AsNoTracking()`. Queries live under `Features/<Area>/Queries/` (suffix `Query`), commands under `Features/<Area>/Commands/` (suffix `Command`). Do not introduce a read/write DbContext split without proven load asymmetry.
- DB access goes through `ICommentMapDbContext` (`CommentMap.Application/Abstractions`), implemented by `CommentMapDbContext` in Infrastructure. Spatial data uses NetTopologySuite `Point` with SRID 3857 against PostGIS. Known accepted CA compromise: `CommentMap.Application` takes a `FrameworkReference` to ASP.NET Core and uses `UserManager`/`SignInManager` directly — wrapping them in abstractions was evaluated and rejected as not worth the churn.
- Email flow: Mvc publishes `CommentMap.Shared/Messages/*` records (they extend `IntegrationEvent`) via `IEventBus.PublishAsync` to the `"commentmap_event_bus"` direct exchange (routing key = type name); EmailSender consumes its single durable queue `"email-sender"` via DI subscriptions (`.AddSubscription<TEvent, THandler>()` in `EmailSender/Program.cs`), renders a RazorBlade `.cshtml` template to MJML (Mjml.Net), and sends via MailKit SMTP (MailPit in dev — check its UI for confirmation emails, since `RequireConfirmedAccount = true`). SMTP settings bind from the `Aspire:Smtp` config section (`SmtpSettings`); TLS is configured via `Security` (MailKit `SecureSocketOptions`, defaults to `None` for MailPit).
- Messaging infra lives in `CommentMap.EventBus` (`IEventBus`, `RabbitMQEventBus`, ported from dotnet/eshop): direct exchange, queue-per-service, keyed-DI handler resolution, System.Text.Json, Polly publish-retry, nack+requeue on handler failure (no DLX yet — see `todos.md`).
- Health endpoints: Mvc maps `/health` and `/alive` in dev via `MapDefaultEndpoints()` (ServiceDefaults). EmailSender is a headless worker — no HTTP endpoints.

## EF Core / Migrations

- Add a migration: `dotnet ef migrations add <Name> --project src/CommentMap.Infrastructure --startup-project src/CommentMap.Mvc`
- Do NOT run `dotnet ef database update` locally — MigrationService applies migrations automatically at startup.

## Conventions & Gotchas

- No test projects, no lint/format config — verification = build + run via AppHost. CI: `.github/workflows/publish-container.yml` builds & pushes the Mvc container image to GHCR on push to `master` (no Dockerfile — uses `dotnet publish /t:PublishContainer`; npm/esbuild run on the runner via the `BuildJS` csproj target).
- `opencode.json` configures the C# LSP via the local `roslyn-language-server` tool; run `dotnet tool restore` first or the LSP won't start.
- Solution file is `CommentMap.slnx` (XML format), not `.sln`.

## Agent skills

### Issue tracker

Issues live in GitHub Issues on `adedw/CommentMap` (via `gh` CLI). See `docs/agents/issue-tracker.md`.

### Triage labels

Default five-role vocabulary (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`). See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: `CONTEXT.md` + `docs/adr/` at the repo root. See `docs/agents/domain.md`.
