# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Fair-Tourney is a participant tournament management application consisting of a React/TypeScript participant-facing web app and an ASP.NET Core 10 backend API. Both are in early development — the current code is mostly scaffold/template.

## Commands

### Frontend (`participant-web-app/`)

```bash
npm run dev       # Start Vite dev server with HMR
npm run build     # Type-check and build production bundle
npm run lint      # Run ESLint
npm run preview   # Preview production build locally
```

### Backend (`backend/`)

```bash
dotnet build                          # Build the whole solution (backend.slnx)
dotnet run --project src/Api          # Start the API server
dotnet watch --project src/Api        # Run with file watching (hot reload)
docker compose up --build             # Build image and run API in Docker at http://localhost:5128
docker build -t fair-tourney-api .    # Build image only
```

The backend runs on `http://localhost:5128` (http profile) or `https://localhost:7214` (https profile), as defined in `backend/src/Api/Properties/launchSettings.json`.

## Architecture

**Two independent apps, no shared code:**

- `participant-web-app/` — React 19 + TypeScript + Vite. Entry: `src/main.tsx` → `src/App.tsx`. Uses Rolldown bundler via Vite. TypeScript is strict (`noUnusedLocals`, `noUnusedParameters`).

- `backend/` — ASP.NET Core 10 Web API using Clean Architecture: one project per layer under `src/`, grouped by `backend.slnx`. Nullable reference types and implicit usings are on.
  - `src/Api` — presentation/startup project. Entry: `Program.cs`; holds appsettings, launch settings and controllers (controller-per-resource under `Controllers/`). OpenAPI docs are enabled in Development only. References Application and Infrastructure.
  - `src/Application` — use cases and interfaces (e.g. repository abstractions). References Domain only.
  - `src/Domain` — entities and business rules. References nothing.
  - `src/Infrastructure` — implementations of Application interfaces (persistence, external services). References Application.
  - Each of Application and Infrastructure exposes a `DependencyInjection.cs` extension (`AddApplication()`, `AddInfrastructure(configuration)`) called from `Program.cs`. Never add references that point outward (e.g. Application → Infrastructure); put EF Core and other infra packages only in Infrastructure.

There is no API proxy configured between the frontend and backend yet — they are developed and run independently.
