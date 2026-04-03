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

## Architecture

- `TaskTrack.Domain`
  - Entities and enums
- `TaskTrack.Application`
  - DTOs, interfaces, and business services
- `TaskTrack.Infrastructure`
  - SQLite access via ADO.NET, password hashing, JWT token creation, and database seeding
- `TaskTrack.WebApi`
  - Controllers, dependency injection, authentication, and HTTP configuration
- `tests/*`
  - Unit tests for services, repositories, and controllers
- `frontend/tasktrack-web`
  - React frontend

## Technical Decisions

- .NET 8 Web API
- SQLite for persistence
- ADO.NET with `Microsoft.Data.Sqlite`
- JWT bearer authentication
- xUnit for tests
- React for the frontend

## Delivery Order

1. Create Clean Architecture project structure
2. Implement domain model and DTO contracts
3. Implement business services and validation
4. Implement SQLite repositories without EF or Dapper
5. Implement authentication and seeding
6. Expose API endpoints
7. Add unit tests
8. Add frontend
9. Write README and presentation notes

## Demo Data

- Demo user: `demo@tasktrack.local`
- Demo password: `Demo123!`

## Presentation Talking Points

- Why task management is a good fit for CRUD plus auth
- How the layers depend inward only
- Why ADO.NET was chosen to meet the constraints
- How tests protect the service layer and repositories
- How GenAI was used critically rather than blindly
