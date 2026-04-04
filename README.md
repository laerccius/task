# TaskTrack

TaskTrack is a full-stack task management application created for a .NET technical interview exercise. It demonstrates CRUD operations, user registration and login, protected and public API endpoints, manual data access with SQLite, Clean Architecture, unit tests, and Aspire-based local orchestration for both the backend and frontend.

## Stack

- Backend: ASP.NET Core Web API on .NET 10
- Architecture: Clean Architecture
- Data access: ADO.NET with `Microsoft.Data.Sqlite`
- Authentication: JWT bearer tokens
- Orchestration: .NET Aspire AppHost on .NET 10
- Frontend: React + Vite
- Tests: xUnit

## Solution Structure

### Backend

- `src/api/domain`
  - Core entities and enums
- `src/api/application`
  - DTOs, interfaces, business rules, and service logic
- `src/api/infrastructure`
  - SQLite repositories, hashing, JWT generation, and database initialization
- `src/api/web-api`
  - Controllers, dependency injection, auth setup, and startup
- `src/api/service-defaults`
  - Shared Aspire service defaults for health checks, telemetry, and resilience

### Orchestration

- `src/aspire/app-host`
  - Aspire AppHost that starts the Web API and the React frontend together

### Frontend

- `src/frontend`
  - React + Vite application for authentication and task CRUD

### Tests

- `tests/api/application.tests`
- `tests/api/infrastructure.tests`
- `tests/api/web-api.tests`

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
- Aspire startup flow for both API and frontend

## Demo Credentials

- Email: `demo@tasktrack.local`
- Password: `Demo123!`

## How To Run

### Prerequisites

- .NET 10 SDK
- Node.js 18+
- npm

### Recommended Startup

Run the full stack through Aspire:

```bash
cd task/src/aspire/app-host
dotnet restore
dotnet run
```

This AppHost starts:

- the Web API from `src/api/web-api`
- the React frontend from `src/frontend`

The frontend receives the API base URL through `VITE_API_URL` from Aspire.

### Direct API Startup

If you want to run only the backend:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task/src/api/web-api
dotnet restore
dotnet run
```

### Direct Frontend Startup

If you want to run only the frontend:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task/src/frontend
npm install
npm run dev
```

If you run the frontend outside Aspire, it falls back to `http://localhost:5050` when `VITE_API_URL` is not set. You can still override that variable explicitly if your backend is running elsewhere.

## Solution File

The repository uses [`TaskTrack.slnx`](/mnt/c/Users/Laerccius/Documents/projects/task/TaskTrack.slnx) and the default startup project is the Aspire AppHost.

## Runtime Requirement

This repository targets .NET 10 for the AppHost and backend projects. To build and run the solution successfully, the machine must have the .NET 10 SDK installed.

## Dependency Management

NuGet package versions are centralized with Central Package Management in [`Directory.Packages.props`](/mnt/c/Users/Laerccius/Documents/projects/task/Directory.Packages.props). Project files keep only package names, which helps avoid version drift across the solution.

## Testing

Run the test suite from the repository root:

```bash
cd /mnt/c/Users/Laerccius/Documents/projects/task
dotnet test TaskTrack.slnx
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

I did not execute the project in this environment while updating the documentation, so the markdown now reflects the current repository structure and startup flow based on the checked-in files. The next verification step should be to run the AppHost and confirm the frontend and backend both come up correctly.
