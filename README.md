# TaskTrack

TaskTrack is a small full-stack task management application created for a .NET technical interview exercise. It demonstrates CRUD operations, user registration and login, protected and public API endpoints, manual data access with SQLite, clean architecture, and unit tests.

## Stack

- Backend: .NET 8 ASP.NET Core Web API
- Architecture: Clean Architecture
- Data access: ADO.NET with `Microsoft.Data.Sqlite`
- Authentication: JWT bearer tokens
- Frontend: React + Vite
- Tests: xUnit

## Architecture

### Backend layers

- `src/TaskTrack.Domain`
  - Core entities and enums
- `src/TaskTrack.Application`
  - DTOs, interfaces, business rules, and service logic
- `src/TaskTrack.Infrastructure`
  - SQLite repositories, hashing, JWT generation, and database initialization
- `src/TaskTrack.WebApi`
  - Controllers, dependency injection, auth setup, and startup
- `src/TaskTrack.ServiceDefaults`
  - Shared Aspire service defaults for health checks, telemetry, and resilience
- `src/TaskTrack.AppHost`
  - Aspire orchestration entry point for local startup

### Frontend

- `frontend/tasktrack-web`
  - Login, registration, and task CRUD UI

## User Story

As a busy professional, I want to register, log in, and manage my personal tasks so I can stay on top of deadlines and work status.

## Features

- Register a user
- Log in and receive a JWT token
- Public auth endpoint and protected user endpoint
- Create, list, update, and delete tasks
- Seeded demo user and tasks
- Business validation in the application layer
- Repository layer without Entity Framework, Dapper, or MediatR

## Demo Credentials

- Email: `demo@tasktrack.local`
- Password: `Demo123!`

## How To Run

### Prerequisites

- .NET SDK 8.0+
- Node.js 18+

### Backend

Recommended startup with Aspire:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task/src/TaskTrack.AppHost
dotnet restore
dotnet run
```

Fallback direct API startup:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task/src/TaskTrack.WebApi
dotnet restore
dotnet run
```

The API is configured to run on [http://localhost:5050](http://localhost:5050).

### Aspire Notes

- `TaskTrack.AppHost` is the main entry point for local orchestration
- `TaskTrack.ServiceDefaults` centralizes health checks and OpenTelemetry wiring
- The first service registered in the AppHost is the existing Web API
- The frontend remains a separate Vite app for now and should still be started with `npm run dev`

### Frontend

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task/frontend/tasktrack-web
npm install
npm run dev
```

The frontend calls the backend at `http://localhost:5050/api`.

## Testing

Run the test suite from the repository root:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task
dotnet test
```

## Seed Data

On application startup, the SQLite database is initialized and seeded with:

- one demo user
- two demo tasks owned by that user

## API Summary

### Public endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/public-info`

### Protected endpoints

- `GET /api/auth/me`
- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

## Testing Strategy

- Application tests validate business rules and edge cases
- Infrastructure tests validate SQLite persistence behavior
- Web API tests validate controller responses

## GenAI Notes

The prompt and review notes used for the GenAI portion of the interview are documented in [`docs/genai-notes.md`](/mnt/c/Users/Laerccius/Documents/projects/task/docs/genai-notes.md).

## Presentation Notes

- Implementation plan: [`docs/implementation-plan.md`](/mnt/c/Users/Laerccius/Documents/projects/task/docs/implementation-plan.md)
- Presentation outline: [`docs/presentation-outline.md`](/mnt/c/Users/Laerccius/Documents/projects/task/docs/presentation-outline.md)

## Important Note

This workspace did not have `dotnet` or `node` installed while the project was scaffolded, so the code and tests were prepared statically but not executed in this environment. Once the SDKs are available, the first step should be `dotnet restore`, `dotnet test`, and `npm install` to validate and adjust anything compiler-specific.
