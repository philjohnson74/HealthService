# HealthService - Patient API

A minimal ASP.NET Core Web API for managing patient records, built as an MVP for a tech test submission.

## Overview

The Patient API provides a simple HTTP interface for retrieving patient data within a health service context. It is structured with future extensibility in mind — the solution is organised so that additional domain services (e.g. appointments, prescriptions) could be added as separate projects under the same solution.

## Project Structure

```
HealthService/
├── src/
│   └── Patient/
│       └── HealthService.Patient.Api/   # Minimal API project
│           ├── Data/                    # Repository interface and in-memory implementation
│           └── Models/                  # Patient domain model
└── HealthService.slnx                   # Solution file
```

## Endpoints

| Method | Route              | Description                  |
|--------|--------------------|------------------------------|
| GET    | `/patients/{id}`   | Retrieve a patient by ID     |

### Example Response — 200 OK

```json
{
  "id": 1,
  "nhsNumber": "ABC123456",
  "name": "Alice Hartley",
  "dateOfBirth": "1985-04-12T00:00:00",
  "gpPractice": "Riverside Medical Centre"
}
```

### Example Response — 404 Not Found

```json
{
  "message": "Patient with ID 99 was not found."
}
```

## Key Design Decisions

### Minimal API over Controller-Based API

The project uses ASP.NET Core's [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) pattern rather than the traditional MVC controller approach. This is a deliberate choice to align with modern .NET conventions: endpoints are defined concisely in `Program.cs` using `MapGet()`, reducing boilerplate and making the routing intent immediately visible.

### Correct HTTP Semantics

Endpoints return appropriate HTTP status codes rather than always returning 200:

- `200 OK` — patient found and returned
- `404 Not Found` — no patient exists for the given ID, with a descriptive JSON error body

This makes the API predictable and easy to consume correctly.

### In-Memory Repository

Data is stored in memory via `InMemoryPatientRepository`, seeded with a small set of example patients. This keeps the MVP self-contained with no external dependencies (no database setup required to run). The repository is abstracted behind an `IPatientRepository` interface, so swapping in a real persistence layer (e.g. Entity Framework Core) is a straightforward step.

### OpenAPI Support

OpenAPI is configured out of the box (via `Microsoft.AspNetCore.OpenApi`), exposed in the Development environment. This provides automatic API documentation and makes the API immediately explorable.

### Dependency Injection

The repository is registered with ASP.NET Core's built-in DI container and injected directly into endpoint handlers — keeping the code testable and decoupled without reaching for third-party IoC containers.

## Running the API

```bash
dotnet run --project src/Patient/HealthService.Patient.Api
```

The API will be available at:
- HTTP: `http://localhost:5162`
- HTTPS: `https://localhost:7288`

## Tech Stack

- **.NET 10**
- **ASP.NET Core Minimal APIs**
- **Microsoft.AspNetCore.OpenApi**
