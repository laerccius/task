# Presentation Outline

## 1. User Story

Introduce the task management use case and explain why it covers CRUD, auth, validation, and user ownership.

## 2. Architecture

Show the four backend layers and explain dependency direction:

- WebApi -> Application
- Infrastructure -> Application
- Application -> Domain
- Domain -> no dependencies

## 3. Demo Flow

- Register or log in with seeded user
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
