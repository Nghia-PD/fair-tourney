> [!WARNING]
> This repository is in heavy development and does not work yet. Expect missing features, breaking changes and incomplete setup steps.

# Fair-Tourney

A tournament management app for participants. It has two parts that are developed and run separately:

| Folder                 | What it is                                                  |
| ---------------------- | ----------------------------------------------------------- |
| `participant-web-app/` | React 19 + TypeScript + Vite web app for participants       |
| `backend/`             | ASP.NET Core 10 Web API, AWS Lambda functions and local infra |
| `docs/`                | Architecture decision records (`decisions/`) and diagrams (`graph/`) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) and npm
- [Docker](https://www.docker.com/) with Docker Compose
- Optional tools:
  - `dotnet tool install --global dotnet-ef` to add EF Core migrations
  - `dotnet tool install --global Amazon.Lambda.Tools` to package Lambda functions

## Getting started

### Backend

Local infrastructure runs in Docker (see `backend/compose.yaml`):

- **PostgreSQL 18** on `localhost:5433`, database `fair_tourney`, user/password `postgres`/`postgres`
- **Floci**, a local AWS emulator, on `localhost:4566`. Its init scripts in `backend/floci/init/ready.d/` create the S3 bucket, SQS queue and Lambda function.

```bash
cd backend

# Package the screenshot Lambda so Floci can deploy it (optional: without it, uploads still work)
dotnet lambda package --project-location src/Functions/Screenshots -o floci/artifacts/functions.zip

# Run everything (API at http://localhost:5128)
docker compose up --build

# ...or run only the infrastructure and start the API from the SDK
docker compose up -d postgres floci
dotnet run --project src/Api      # http://localhost:5128, https://localhost:7214
dotnet watch --project src/Api    # with hot reload
```

In Development the API applies pending EF Core migrations at startup, and OpenAPI docs are enabled.

To add a migration:

```bash
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Api --output-dir Data/Migrations
```

### Frontend

```bash
cd participant-web-app
npm install
npm run dev       # Vite dev server with HMR
npm run build     # Type-check and build for production
npm run lint      # ESLint
npm run preview   # Preview the production build
```

No proxy is set up between the frontend and the backend yet.

## Backend architecture

The backend follows Clean Architecture with CQRS, based on [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture):

- `src/Domain`: entities and business rules
- `src/Application`: use cases, command/query handlers, validators and interfaces
- `src/Infrastructure`: EF Core (PostgreSQL), storage and other implementations
- `src/Api`: MVC controllers and startup
- `src/Functions`: AWS Lambda functions (for example `Screenshots`, which processes screenshot chunks from SQS)

See [`CLAUDE.md`](CLAUDE.md) for detailed conventions and [`docs/decisions/`](docs/decisions/) for design decisions.
