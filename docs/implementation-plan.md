# Implementation Plan

## Chosen Use Case

Build a task management application where authenticated users can manage their own tasks.

## Informal User Story

As a busy professional, I want to register, log in, and manage my tasks in one place so I can track work that is pending, in progress, or done before the due date.

## Functional Scope

- User registration
- User login with JWT authentication
- Public endpoints for health and auth
- Protected endpoints for task CRUD
- Seeded demo user and demo tasks
- Responsive frontend for authentication and task management
- Aspire-based orchestration for local developer startup

## Architecture

- `src/api/domain`
  - Entities and enums
- `src/api/application`
  - DTOs, interfaces, and business services
- `src/api/infrastructure`
  - SQLite access via ADO.NET, password hashing, JWT token creation, and database seeding
- `src/api/web-api`
  - Controllers, dependency injection, authentication, and HTTP configuration
- `src/api/service-defaults`
  - Shared Aspire defaults such as health checks and telemetry
- `src/aspire/app-host`
  - Orchestration layer for starting backend and frontend together
- `src/frontend`
  - React frontend
- `tests/api/*`
  - Unit tests for services, repositories, and controllers

## Technical Decisions

- ASP.NET Core Web API on .NET 10
- SQLite for persistence
- ADO.NET with `Microsoft.Data.Sqlite`
- JWT bearer authentication
- xUnit for tests
- React + Vite for the frontend
- .NET Aspire AppHost on .NET 10 for local orchestration

## Delivery Order

1. Create Clean Architecture project structure
2. Implement domain model and DTO contracts
3. Implement business services and validation
4. Implement SQLite repositories without EF or Dapper
5. Implement authentication and seeding
6. Expose API endpoints
7. Add unit tests
8. Add frontend
9. Add Aspire orchestration for local startup
10. Write README and presentation notes

## Demo Data

- Demo user: `demo@tasktrack.local`
- Demo password: `Demo123!`

## Presentation Talking Points

- Why task management is a good fit for CRUD plus auth
- How the layers depend inward only
- Why ADO.NET was chosen to meet the constraints
- Why MediatR was intentionally not used
- How Aspire improves startup without changing the core architecture
- How tests protect the service layer and repositories
- How GenAI was used critically rather than blindly
