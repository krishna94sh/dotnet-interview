# Solution Documentation

**Candidate Name:** [Your Name]  
**Completion Date:** [Date]

---

## Problems Identified

After reviewing the original TODO API implementation, I identified the following issues:

### 1. Controller and Service Tight Coupling

**Problem:**

The original controller was creating the service directly:

```csharp
var todoService = new TodoService();
```

This created tight coupling between the API layer and business logic layer.

**Impact:**

- Difficult to unit test controllers
- Difficult to replace service implementations
- Violates separation of concerns

**My Decision:**

I introduced Dependency Injection and changed the controller to depend on an abstraction (`ITodoService`) instead of directly creating `TodoService`.

This improves maintainability and allows easier mocking during testing.

---

### 2. Missing Service Abstraction

**Problem:**

The original implementation only contained a concrete `TodoService` class.

There was no interface defining the service contract.

**Impact:**

- Business logic was tightly coupled
- Testing was harder
- Future changes would require modifying multiple components

**My Decision:**

I created:

```
Services
│
├── ITodoService.cs
└── TodoService.cs
```

The interface defines available operations while the implementation handles the actual logic.

---

### 3. SQL Injection Vulnerability

**Problem:**

The original application used string interpolation while creating SQL queries.

Example:

```csharp
command.CommandText = $"SELECT * FROM Todos WHERE Id = {id}";
```

This could allow malicious SQL input.

**My Decision:**

I replaced dynamic SQL queries with parameterized queries.

Example:

```csharp
command.CommandText = "SELECT * FROM Todos WHERE Id = @id";
command.Parameters.AddWithValue("@id", id);
```

This improves database security.

---

### 4. Poor REST API Design

**Problem:**

The original API used POST requests for all operations:

```
POST /createTodo
POST /getTodo
POST /updateTodo
POST /deleteTodo
```

This does not follow REST API conventions.

**My Decision:**

I redesigned the endpoints using standard HTTP methods:

```
GET     /api/todos
GET     /api/todos/{id}
POST    /api/todos
PUT     /api/todos/{id}
DELETE  /api/todos/{id}
```

This makes the API easier to understand and consume.

---

### 5. No DTO Layer

**Problem:**

The original implementation directly exposed the database model.

This created a strong dependency between database structure and API contracts.

**My Decision:**

I introduced DTO models:

```
Models
│
├── Todo.cs
│
└── DTOs
    ├── CreateTodoRequest.cs
    ├── UpdateTodoRequest.cs
    └── TodoResponse.cs
```

This separates internal database models from external API models.

---

### 6. Limited Testing

**Problem:**

The existing project had limited test coverage.

**My Decision:**

I added unit tests covering:

Positive scenarios:
- Create TODO successfully
- Retrieve TODO by ID
- Retrieve all TODO items
- Update TODO
- Delete TODO

Negative scenarios:
- Retrieve non-existing TODO
- Delete non-existing TODO

---

## Architectural Decisions

### 1. Layered Architecture

**Decision:**

I implemented a layered architecture:

```
Controller Layer
        |
        |
Service Layer
        |
        |
Database Layer
```

### Reason:

Each layer has a clear responsibility.

Controller:
- Handles HTTP requests
- Validates input
- Returns API responses

Service:
- Contains business logic
- Handles TODO operations

Database:
- Stores and retrieves data

This improves maintainability and scalability.

---

### 2. Dependency Injection

**Decision:**

I used ASP.NET Core built-in Dependency Injection.

Service registration:

```csharp
builder.Services.AddScoped<ITodoService, TodoService>();
```

### Reason:

Dependency Injection provides:

- Loose coupling
- Better testability
- Cleaner code structure

---

### 3. DTO Pattern

**Decision:**

I introduced separate DTO classes for requests and responses.

### Reason:

Using DTOs prevents exposing database entities directly and provides flexibility for future API changes.

Example:

Request:

```
CreateTodoRequest
```

Response:

```
TodoResponse
```

---

### 4. RESTful API Design

**Decision:**

I changed the API endpoints to follow REST conventions.

### Reason:

Using correct HTTP verbs makes the API:

- Easier to understand
- More predictable
- Compatible with standard API clients

---

### 5. SQLite Database

**Decision:**

I kept SQLite as the database.

### Reason:

The original project already used SQLite, and changing the database technology was unnecessary for this assessment.

---

## Trade-offs

### 1. Kept SQLite Instead of Migrating to Entity Framework Core

**Decision:**

I continued using SQLite with SQL queries.

### Reason:

The goal was to focus on architecture improvements rather than introducing a completely new data access layer.

### Alternative Considered:

Entity Framework Core.

### Future Consideration:

For a larger production application, EF Core would provide:

- Easier migrations
- Better maintainability
- Cleaner data access

---

### 2. Did Not Implement Repository Pattern

**Decision:**

I did not introduce a repository layer.

### Reason:

The application is small, and adding another abstraction layer would increase complexity.

### Alternative:

A larger system could use:

```
Controller
    |
Service
    |
Repository
    |
Database
```

---

### 3. Basic Exception Handling

**Decision:**

I kept error handling simple within the API layer.

### Reason:

Advanced exception middleware was outside the assessment scope.

### Future Improvement:

Implement centralized exception handling middleware.

---

### 4. Authentication Not Added

**Decision:**

Authentication was not implemented.

### Reason:

The requirement only covered CRUD operations.

### Future Improvement:

Add:

- JWT authentication
- User accounts
- User-specific TODO management

---

## How to Run

### Prerequisites

Required:

- .NET SDK 8 or later
- SQLite

Verify installation:

```bash
dotnet --version
```

---

### Build

Run:

```bash
dotnet build
```

---

### Run

Start the API:

```bash
dotnet run --project TodoApi
```

Swagger UI will be available:

```
https://localhost:<port>/swagger
```

---

### Test

Run all tests:

```bash
dotnet test
```

---

## API Documentation

### Endpoints

#### Create TODO

```
Method:
POST

URL:
/api/todos

Request Body:

{
  "title": "Complete assessment",
  "description": "Finish TODO API refactoring",
  "isCompleted": false
}

Response:

201 Created
```

---

#### Get TODO(s)

```
Method:
GET

URL:

Get all:
/api/todos

Get by ID:
/api/todos/{id}


Response:

200 OK
404 Not Found
```

Example response:

```json
[
  {
    "id":1,
    "title":"Complete assessment",
    "description":"Finish TODO API refactoring",
    "isCompleted":false,
    "createdAt":"2026-07-30T10:00:00"
  }
]
```

---

#### Update TODO

```
Method:
PUT

URL:
/api/todos/{id}

Request Body:

{
  "title":"Updated TODO",
  "description":"Updated description",
  "isCompleted":true
}

Response:

200 OK
404 Not Found
400 Bad Request
```

---

#### Delete TODO

```
Method:
DELETE

URL:
/api/todos/{id}

Response:

200 OK
404 Not Found
400 Bad Request
```

---

## Future Improvements

If I had more time, I would implement the following improvements:

### 1. Repository Pattern

Introduce a repository layer to separate database operations from business logic.

---

### 2. Entity Framework Core

Replace manual SQL queries with Entity Framework Core.

Benefits:

- Easier database migrations
- Cleaner data access
- Better maintainability

---

### 3. Global Exception Middleware

Create centralized exception handling.

Benefits:

- Consistent API error responses
- Less duplicate error handling code

---

### 4. Better Test Isolation

Improve testing by using:

- In-memory database
- Mock repositories
- Test containers

This prevents tests from sharing the same database file.

---

### 5. Logging and Monitoring

Add:

- Structured logging
- Health checks
- Application monitoring

---

### 6. Authentication and Authorization

Implement:

- User registration
- JWT authentication
- User-specific TODO management

