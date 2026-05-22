# TaskManager API

A backend API for managing projects and tasks, built with .NET 9 and Clean Architecture. JWT authentication, CQRS with MediatR, EF Core with SQL Server, and optional Redis caching are all included. It was built to be clean, practical, and easy to run locally or in Docker.

---

## Requirements Checklist

### Required Stack
- ✅ .NET 9
- ✅ ASP.NET Core Web API
- ✅ Entity Framework Core
- ✅ SQL Server
- ✅ JWT Authentication
- ✅ Clean Architecture

### Functional Requirements

**Authentication**
- ✅ Register — `POST /api/v1/auth/register`
- ✅ Login — `POST /api/v1/auth/login`

**Projects Module**
- ✅ Create Project
- ✅ Get All Projects
- ✅ Get Project By Id
- ✅ Update Project
- ✅ Delete Project
- ✅ Project model: `Id`, `Name`, `Description`, `CreatedAt`

**Tasks Module**
- ✅ Create Task
- ✅ Update Task / Task Status
- ✅ Get Tasks By Project
- ✅ Delete Task
- ✅ Task model: `Id`, `Title`, `Description`, `Status`, `DueDate`, `Priority`, `ProjectId`

### Architecture Requirements
- ✅ Clean Architecture (Domain / Application / Infrastructure / API layers)
- ✅ Dependency Injection
- ✅ SOLID Principles
- ✅ DTO Usage
- ✅ Global Exception Handling (`GlobalExceptionHandler` + `ApiResponse<T>` wrapper)
- ✅ Validation (FluentValidation, runs before handlers)

### Bonus Points *(all implemented)*
- ✅ CQRS
- ✅ MediatR
- ✅ Docker (Dockerfile + docker-compose with SQL Server & Redis)
- ✅ Unit Tests (xUnit + FluentAssertions)
- ✅ Redis (distributed cache with in-memory fallback)
- ✅ Generic Response Wrapper (`ApiResponse<T>` on every response)
- ✅ Role-based Authorization (`User` and `Admin` roles)
- ✅ API Versioning (`/api/v1/...`)

### Deliverables
- ✅ GitHub Repository
- ✅ README with setup instructions and architecture overview
- ✅ Database migration files (auto-applied on startup)
- ✅ Swagger UI at `/swagger` + `TaskManager.http` file for testing

---

## Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core 9 Web API |
| ORM | Entity Framework Core 9 (SQL Server) |
| Auth | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / API) |
| CQRS | MediatR |
| Validation | FluentValidation |
| Caching | Redis (distributed) — falls back to in-memory if not configured |
| Logging | Serilog (console sink) + custom HTTP request logging middleware |
| API Docs | Swagger UI (`Swashbuckle.AspNetCore`) |
| API Versioning | `Asp.Versioning.Mvc` |
| Password Hashing | BCrypt.Net-Next |
| Health Checks | EF Core DB health check at `/health` |
| Testing | xUnit + FluentAssertions |
| CI | GitHub Actions |
| Containers | Docker + Docker Compose (SQL Server 2022 + Redis 7) |

---

## Project Structure

```
.
├── src/
│   ├── TaskManager.Domain/           # Entities, enums, base classes
│   ├── TaskManager.Application/      # CQRS commands/queries, handlers, validators, DTOs
│   ├── TaskManager.Infrastructure/   # EF Core, repositories, JWT, caching, password hashing
│   └── TaskManager.API/              # Controllers, middleware, filters, DI wiring
├── tests/
│   └── TaskManager.Application.Tests/  # Unit tests for handlers and validators
├── Dockerfile
├── docker-compose.yml
└── TaskManager.http                  # .http file for quick API testing
```

---

## Getting Started (Local)

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or Docker)
- Redis (optional — app falls back to in-memory cache if not configured)

### 1. Clone and Restore

```bash
git clone <repository-url>
cd "Electro Pi - .Net Task"
dotnet restore
```

### 2. Configure the Connection String

Edit `src/TaskManager.API/appsettings.json` if your SQL Server instance differs:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskManagerDb;Trusted_Connection=true;Encrypt=false;",
    "Redis": ""
  }
}
```

Leave `Redis` empty to use the in-memory fallback cache. Set it to `localhost:6379` if you have Redis running locally.

### 3. Run the API

```bash
dotnet run --project src/TaskManager.API/TaskManager.API.csproj
```

Migrations and the admin seed are applied automatically on startup. When it boots, you'll see the Swagger link logged:

```
[HH:mm:ss INF] API Documentation is available at: https://localhost:5001/swagger
```

Open that URL in your browser to explore and test all endpoints interactively.

---

## Running with Docker

The compose file brings up SQL Server, Redis, and the API together:

```bash
docker compose up --build
```

- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

To stop everything:

```bash
docker compose down
```

---

## API Endpoints

All endpoints are versioned under `/api/v1`.

### Authentication

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/v1/auth/register` | Register a new user (receives `User` role) |
| `POST` | `/api/v1/auth/login` | Login and receive a JWT token |

### Admin *(requires `Admin` role)*

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/v1/admin/users/count` | Total number of registered users |

### Projects *(requires authentication)*

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/v1/projects` | Create a project |
| `GET` | `/api/v1/projects` | Get all your projects |
| `GET` | `/api/v1/projects/{projectId}` | Get a specific project |
| `PUT` | `/api/v1/projects/{projectId}` | Update a project |
| `DELETE` | `/api/v1/projects/{projectId}` | Delete a project |

### Tasks *(requires authentication)*

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/v1/projects/{projectId}/tasks` | Create a task |
| `GET` | `/api/v1/projects/{projectId}/tasks` | Get all tasks in a project |
| `GET` | `/api/v1/projects/{projectId}/tasks/{taskId}` | Get a specific task |
| `PUT` | `/api/v1/projects/{projectId}/tasks/{taskId}` | Update a task |
| `DELETE` | `/api/v1/projects/{projectId}/tasks/{taskId}` | Delete a task |

### Other

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/health` | Database health check |
| `GET` | `/metrics` | Simple cache counters (hits / misses / invalidations) |

---

## Default Admin Account

A seed admin account is created on first run:

| Email | Password | Role |
|-------|----------|------|
| `admin@taskmanager.local` | `Admin123!` | Admin |

Regular users sign up via `POST /api/v1/auth/register`.

---

## Testing

### Using the `.http` file

Open `TaskManager.http` in Visual Studio or VS Code (with the REST Client extension). It covers all endpoints. Workflow:

1. Run the **Login as Admin** or **Register + Login** request.
2. Copy the `token` from the response.
3. Paste it into the `@Token` variable at the top of the file.
4. Run any other request.

### Using Swagger

Navigate to `/swagger`, click **Authorize**, and paste your JWT token. All authenticated endpoints will work from there.

### Unit Tests

```bash
dotnet test
```

Tests are in `tests/TaskManager.Application.Tests` and cover handlers and validators.

---

## Key Design Decisions

**Clean Architecture** — The domain has no external dependencies. Application only knows about domain abstractions. Infrastructure implements them. The API layer wires everything together and stays thin.

**CQRS with MediatR** — Reads and writes are separated into queries and commands. Each handler is small and focused, making the codebase easy to extend.

**Caching** — Read queries are cached per-user. Write commands invalidate the relevant cache entries. The cache backend is Redis when configured, otherwise falls back to the built-in distributed memory cache — no code changes needed.

**Soft deletes** — Records are never hard-deleted. `IsDeleted = true` is set instead, and EF Core query filters exclude them automatically.

**Audit fields** — `CreatedAt` and `UpdatedAt` are set automatically via an EF Core interceptor.

**Consistent API responses** — Every response (success or error) is wrapped in an `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Project retrieved successfully",
  "data": { ... },
  "errors": []
}
```




---

## Continuous Integration

A GitHub Actions workflow at `.github/workflows/ci.yml` runs on every push and pull request:

- `dotnet restore` + `dotnet build`
- `dotnet test`
- `docker build` (validates the Dockerfile)

---


