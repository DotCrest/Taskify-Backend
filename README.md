# Taskify Backend

ASP.NET Core **.NET 8** backend for a task/workspace collaboration system.  
Built with a layered architecture (**Domain / Application / Infrastructure / WebApi**) and includes JWT auth, background jobs, caching, real-time SignalR messaging, and unit tests.

---

## Tech Stack

- **.NET / ASP.NET Core**: net8.0
- **Database**: SQL Server (Entity Framework Core)
- **Auth**: ASP.NET Core Identity + **JWT Bearer**
- **Background Jobs**: **Hangfire** (SQL Server storage + Dashboard)
- **Realtime**: **SignalR**
- **Caching**: **Redis** (StackExchange.Redis)
- **Email**: MailKit + **Polly retry policy**
- **Validation**: FluentValidation
- **Dynamic querying**: System.Linq.Dynamic.Core (for dynamic sort)
- **Testing**: xUnit, Moq, Bogus, FluentAssertions

---

## Solution Structure

- `WebApi/`  
  ASP.NET Core host project (controllers, SignalR hubs, program setup, Swagger, Hangfire dashboard).

- `Application/`  
  Application services, DTOs, validators, specifications, shared utilities (Result pattern, pagination models).

- `Infrastructure/`  
  EF Core context & migrations, repositories + Unit Of Work, cache + email implementations, seeding.

- `Domain/`  
  Domain models, contracts (interfaces), settings/options, constants.

- `Application.Tests/`  
  Unit tests + fakers (Bogus), mocking (Moq), assertions (FluentAssertions).

---

## Key Features

### Authentication & Authorization (JWT)
- JWT Bearer authentication configured in `WebApi/Program.cs`.
- ASP.NET Core Identity for user/role management.
- SignalR token support via query-string token extraction for hubs.

### Invitation Service
- Invitation workflow is implemented in `Application/Services/InvitationService.cs` and exposed via `WebApi/Controllers/InvitationController.cs`.
- Supports creating and managing workspace invitations, with automated cleanup/update via a Hangfire recurring job (expired invitations).

### Background Jobs (Hangfire)
- Hangfire server + dashboard enabled.
- Recurring jobs configured in `WebApi/Program.cs` (e.g., expired invitations cleanup).

### Email Service (Retry Policy)
- `Infrastructure/EmailService.cs` uses **Polly WaitAndRetryAsync** with exponential backoff
  to handle transient SMTP/network failures.

### Repository Pattern + Unit Of Work
- Generic repository abstraction with async CRUD.
- `Infrastructure/Repository/UnitOfWork.cs` provides:
  - `Repository<TEntity>()` repository resolver
  - `ExecuteInTransactionAsync(...)` transactional wrapper
  - `SaveAsync()` for persistence

### Specification Pattern + Pagination + Dynamic Sort
- `Application/Specifications/BaseSpecification.cs` supports:
  - Criteria filtering
  - Includes
  - Pagination (skip/take)
  - Dynamic sorting with allow-listed properties
- `Infrastructure/SpecificationEvaluator.cs` applies specification to EF queries.
- Shared pagination models live under `Application/Shared/Pagination/`.

### Caching (Redis)
- `Infrastructure/CacheService.cs` implements Redis caching.
- `WebApi/ActionFilters/CacheAttribute.cs` caches successful GET responses based on path + query string.

### Real-time Comments (SignalR)
- SignalR hub hosted at:
  - `/hub/comments`
- Hub types are under `WebApi/Hubs/`.

### Result Pattern
- `Application/Shared/Result.cs` provides a generic Result wrapper to standardize success/failure responses
  and error handling in the application layer.

### Unit Testing
- `Application.Tests` uses:
  - **xUnit**
  - **Moq**
  - **Bogus**
  - **FluentAssertions**

---

## Getting Started

### Prerequisites
- .NET SDK **8.x**
- SQL Server instance (local or container)
- Redis instance

### Configuration

Update connection strings and options in:

- `WebApi/appsettings.json`
- `WebApi/appsettings.Development.json`

You will typically need:
- `DefaultConnectionString` (SQL Server)
- `RedisConnectionString`
- `email-config` (SMTP host/port/email/password)
- `JwtOptions` (Issuer, Audience, SecretKey)

> Note: The repo uses ASP.NET Core User Secrets in `WebApi` as well.

---

## Run the API

From repository root:

```bash
dotnet restore
dotnet build
dotnet run --project WebApi/WebApi.csproj
```

Swagger is enabled in Development.

---

## Hangfire Dashboard

The Hangfire dashboard is mapped at:

- `/hangfire`

---

## SignalR

Comment hub endpoint:

- `/hub/comments`

When using JWT with SignalR, the token can be passed via query string as `access_token`
(see JWT bearer `OnMessageReceived` in `WebApi/Program.cs`).

---

## Running Tests

```bash
dotnet test
```

---

## Notes / (TODO)

- Dockerfile / docker-compose
