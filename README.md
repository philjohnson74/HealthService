# HealthService - Patient API

A minimal ASP.NET Core Web API for managing patient records, built as an MVP for a tech test submission.

## Overview

The Patient API provides a simple HTTP interface for retrieving patient data within a health service context. It is structured with future extensibility in mind — the solution is organised so that additional domain services (e.g. appointments, prescriptions) could be added as separate projects under the same solution.

## Project Structure

```
HealthService/
├── src/
│   └── Patient/
│       └── HealthService.Patient.Api/
│           ├── Data/                    # Repository interface and in-memory implementation
│           ├── Endpoints/               # Endpoint registration extension methods
│           ├── Models/                  # Patient domain model
│           └── Services/               # Service layer (IPatientService / PatientService)
├── tests/
│   └── Patient/
│       └── HealthService.Patient.Api.Tests/   # xUnit test project
│           ├── Data/                          # Repository tests
│           └── Services/                      # Service layer tests
└── HealthService.slnx
```

## Endpoints

| Method | Route              | Description                  |
|--------|--------------------|------------------------------|
| GET    | `/patients/{id}`   | Retrieve a patient by ID     |

### Example Response — 200 OK

```json
{
  "id": 1,
  "nhsNumber": "485 777 3456",
  "name": "Alice Hartley",
  "dateOfBirth": "1985-03-12T00:00:00",
  "gpPractice": "Northside Medical Centre"
}
```

### Example Response — 404 Not Found

```json
{
  "message": "Patient with ID 99 was not found."
}
```

## Key Design Decisions

### Minimal API with Clean Separation of Concerns

The project uses ASP.NET Core's [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) pattern rather than traditional MVC controllers. A common pitfall with Minimal APIs is letting `Program.cs` become a dumping ground — so endpoint registration is extracted into a `PatientEndpoints` static extension class, keeping `Program.cs` as thin wiring only. This demonstrates understanding of the pattern and its trade-offs.

### Service Layer

A `PatientService` sits between the endpoint handlers and the repository, behind an `IPatientService` interface. This gives the endpoints a clean dependency to program against and a natural home for any business logic that accumulates over time (validation, enrichment, authorisation checks). It also makes the endpoint handlers independently testable without needing to stand up a full HTTP stack.

### Correct HTTP Semantics

Endpoints return appropriate HTTP status codes rather than always returning 200:

- `200 OK` — patient found and returned
- `404 Not Found` — no patient exists for the given ID, with a descriptive JSON error body

This makes the API predictable and easy to consume correctly.

### In-Memory Repository

Data is stored in memory via `InMemoryPatientRepository`, seeded with a small set of example patients. This keeps the MVP self-contained with no external dependencies (no database setup required to run). The repository is abstracted behind an `IPatientRepository` interface, so swapping in a real persistence layer (e.g. Entity Framework Core) is a straightforward step.

### OpenAPI and Scalar API Reference

OpenAPI is configured out of the box (via `Microsoft.AspNetCore.OpenApi`), and [Scalar](https://github.com/scalar/scalar) is wired up to provide an interactive API reference UI — both are exposed in the Development environment only.

Scalar is a modern alternative to Swagger UI. It reads the generated OpenAPI spec and presents a browser-based interface where developers can browse endpoints, inspect request/response schemas, and execute live requests against the running API without needing a separate tool like Postman or curl.

### Dependency Injection

Services and repositories are registered with ASP.NET Core's built-in DI container and injected into endpoint handlers and services — keeping the code testable and decoupled without reaching for third-party IoC containers.

## Running the API

```bash
dotnet run --project src/Patient/HealthService.Patient.Api
```

The API will be available at:
- HTTP: `http://localhost:5162`
- HTTPS: `https://localhost:7288`

### Interactive API Reference (Scalar)

With the API running in Development, open the Scalar UI in your browser:

```
http://localhost:5162/scalar/v1
```

From there you can:
- Browse the available endpoints and their expected request/response shapes
- Send live requests directly from the browser — no Postman or curl required
- Inspect the raw OpenAPI spec at `http://localhost:5162/openapi/v1.json`

## Running the Tests

```bash
dotnet test
```

Or to run only the Patient API tests:

```bash
dotnet test tests/Patient/HealthService.Patient.Api.Tests
```

Tests are written with [xUnit](https://xunit.net/) and [Moq](https://github.com/devlooped/moq). The test project targets the service layer, using a mocked `IPatientRepository` to verify behaviour in isolation.

## Tech Stack

- **.NET 10**
- **ASP.NET Core Minimal APIs**
- **Microsoft.AspNetCore.OpenApi**
- **Scalar** (interactive API reference UI)
- **xUnit** + **Moq** (testing)
