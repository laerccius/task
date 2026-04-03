# GenAI Notes

## Prompt Used

```text
Generate a .NET 8 ASP.NET Core Web API for a task management system using Clean Architecture.
Constraints:
- Do not use Entity Framework, Dapper, or MediatR
- Use SQLite with ADO.NET and Microsoft.Data.Sqlite
- Add JWT authentication
- Implement registration and login
- Implement CRUD endpoints for tasks
- A task has id, userId, title, description, status, and dueDate
- Separate Domain, Application, Infrastructure, and WebApi layers
- Add unit tests for services, repositories, and controllers
- Keep business validation inside the application layer
- Seed a demo user and a few demo tasks
Return representative code for Program.cs, controllers, repositories, services, and tests
```

## How To Present GenAI Usage

- I used AI to accelerate scaffolding and compare implementation options.
- I validated each suggestion against the exercise constraints, especially the ban on Entity Framework, Dapper, and Mediator.
- I corrected generated code where it blurred boundaries between the application and infrastructure layers.
- I tightened validation around empty titles, invalid due dates, cross-user task access, and login failures.
- I kept authentication explicit and verified that only public auth endpoints allow anonymous access.

## Representative Improvements

- Replaced ORM-style repository examples with parameterized SQLite commands
- Moved validation out of controllers and into application services
- Ensured task queries are scoped by authenticated user id
- Added deterministic tests around invalid input and unauthorized access
