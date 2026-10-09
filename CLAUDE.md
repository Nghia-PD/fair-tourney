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
dotnet run        # Start the API server
dotnet build      # Build the project
dotnet watch      # Run with file watching (hot reload)
```

The backend runs on `http://localhost:5128` (http profile) or `https://localhost:7214` (https profile), as defined in `backend/Properties/launchSettings.json`.

## Architecture

**Two independent apps, no shared code:**

- `participant-web-app/` — React 19 + TypeScript + Vite. Entry: `src/main.tsx` → `src/App.tsx`. Uses Rolldown bundler via Vite. TypeScript is strict (`noUnusedLocals`, `noUnusedParameters`).

- `backend/` — ASP.NET Core 10 Web API. Entry: `Program.cs`. Controller-per-resource pattern under `Controllers/`. OpenAPI docs are enabled in Development only. Nullable reference types and implicit usings are on.

There is no API proxy configured between the frontend and backend yet — they are developed and run independently.
