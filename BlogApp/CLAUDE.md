# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the app (HTTP)
dotnet run --launch-profile http

# Build
dotnet build

# Apply EF Core migrations
dotnet ef database update

# Add a new migration
dotnet ef migrations add <MigrationName>
```

## Architecture

ASP.NET Core MVC app targeting .NET 10, backed by PostgreSQL via EF Core (Npgsql).

**Key layers:**
- `Models/` — domain entities (`Post`, `PostStatus` enum) and view models
- `Data/AppDbContext.cs` — EF Core context; `Post` entity is configured here (unique slug index, `PostStatus` stored as string)
- `Controllers/` — MVC controllers
- `Views/` — Razor views with shared layout in `Views/Shared/_Layout.cshtml`

**Database connection** is configured via `ConnectionStrings:DefaultConnection` in `appsettings.json` (not present by default — supply via user secrets or environment variable). In development, use `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>"`.

**Post model invariants:**
- `Slug` must be unique (enforced at the DB level via a unique index)
- `PostStatus` is persisted as a string (`"Draft"`, `"Published"`, `"Archived"`), not an integer

**Docker:** A multi-stage `Dockerfile` is included; exposes ports 8080 (HTTP) and 8081 (HTTPS).
