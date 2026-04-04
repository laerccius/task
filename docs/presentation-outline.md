# Presentation Outline

## 1. User Story

Introduce the task management use case and explain why it covers CRUD, auth, validation, and user ownership.

## 2. Architecture

Show the backend layers and explain dependency direction:

- Web API -> Application
- Infrastructure -> Application
- Application -> Domain
- Domain -> no dependencies

Then show the developer-experience layer:

- AppHost -> orchestrates Web API and frontend locally
- Service Defaults -> shared cross-cutting runtime concerns

## 3. Demo Flow

- Start the solution from the Aspire AppHost
- Mention that the solution targets .NET 10 before starting the demo
- Open the frontend
- Log in with the seeded user
- View seeded tasks
- Create a task
- Update its status
- Delete a task
- Show that protected endpoints require a token

## 4. Testing

- Service tests for business rules
- Repository tests for persistence
- Controller tests for endpoint behavior

## 5. GenAI Usage

- Show the prompt
- Explain what was accepted, changed, and rejected
- Explain how architectural and validation issues were corrected

## 6. Architecture Talking Point

Explain that Aspire improves local startup and orchestration, but it does not replace Clean Architecture. The domain, application, infrastructure, and web layers still carry the architectural boundaries.
