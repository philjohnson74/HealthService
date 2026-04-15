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
│           ├── Integration/                   # WebApplicationFactory integration tests
│           ├── Models/                        # DTO mapping tests
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
  "dateOfBirth": "1985-03-12",
  "gpPractice": "Northside Medical Centre"
}
```

### Example Response — 404 Not Found

```json
{
  "message": "Patient with ID 99 was not found."
}
```

> The error body is typed as `ErrorResponse` — a named record rather than an anonymous object — so its schema is visible in the Scalar API reference alongside the 200 response.

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

**`TypedResults` and `.Produces<>()`**

The endpoint uses `TypedResults` rather than the non-generic `Results`. This provides two concrete benefits:

- **Compile-time safety** — the compiler enforces that the handler returns one of the declared result types, catching mismatches before runtime
- **Accurate OpenAPI schema inference** — ASP.NET Core can statically analyse the `Results<Ok<PatientResponse>, NotFound<ErrorResponse>>` return type and generate precise response schemas automatically

`.Produces<PatientResponse>(200)` and `.Produces<ErrorResponse>(404)` are also declared explicitly on the endpoint, ensuring Scalar displays the correct response shapes for both outcomes — not just `object`.

### Response DTO (`PatientResponse`)

The API returns a `PatientResponse` DTO rather than the `Patient` domain model directly. This creates an explicit, stable contract between the API and its consumers, decoupled from the internal domain model.

**Why this matters for the current GET endpoint:**
- **Security** — any field added to the `Patient` domain model in the future (audit timestamps, soft-delete flags, internal notes) is not automatically exposed to callers. The response shape is opt-in, not opt-out.
- **Shaping** — the DTO can present data differently from how it is stored. For example, `PatientResponse` exposes `DateOfBirth` as `DateOnly` (a date string with no time component) rather than the `DateTime` held on the domain model — which is more semantically correct for a date of birth and avoids a misleading `T00:00:00` suffix in the response.

**Benefits if a `POST /patients` or `PUT /patients/{id}` endpoint were added:**

A separate `CreatePatientRequest` or `UpdatePatientRequest` DTO would provide:
- **Validation** — attributes like `[Required]` and `[Range]`, or a FluentValidation ruleset, can be applied to the incoming DTO without polluting the domain model.
- **Over-posting protection** — callers cannot supply fields like `Id` that should only be set by the system. The request DTO exposes only what the API intentionally accepts.
- **Separation of concerns** — the shape of what is written can differ from what is read, without compromising the domain model or leaking internal structure.

### Logging

`PatientService` uses `ILogger<PatientService>`, injected via the constructor, to log meaningful events during request handling:

| Event | Level | Rationale |
|---|---|---|
| Patient not found | `Warning` | Not an application error, but a pattern of 404s could indicate a bad client, stale references, or probing behaviour — worth surfacing above the noise floor |
| Patient retrieved | `Debug` | Happy-path confirmation useful for tracing a specific request, but too noisy for production — disabled by default |

Structured logging placeholders (`{PatientId}`) are used rather than string interpolation, so log values are queryable as first-class fields in tools like Seq or Application Insights.

The `LogDebug` call is guarded with `IsEnabled(LogLevel.Debug)` to avoid evaluating arguments when debug logging is disabled — a minor but correct performance consideration.

Unhandled exceptions are not explicitly logged here because ASP.NET Core's built-in exception handling middleware already captures and logs these at `Error` level — adding a second log would be duplication.

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

## Testing

### The Testing Pyramid

This project follows the testing pyramid — many fast, focused unit tests at the base, with fewer but broader integration tests above.

**Unit tests** verify individual classes in complete isolation. Dependencies are replaced with Moq mocks, there is no I/O, and tests run in milliseconds. They are the right tool for testing business logic, mapping, and edge cases in a single component.

**Integration tests** verify the full HTTP pipeline end-to-end using `WebApplicationFactory<Program>`, which spins up the real application in-process. They test things unit tests cannot: that routing is wired correctly, that JSON serialisation produces the right output over the wire, that middleware behaves as expected, and that all the components work together correctly. They are slower than unit tests and used more sparingly.

| Layer | What is tested | Tool |
|---|---|---|
| Unit | `PatientService`, `InMemoryPatientRepository`, `PatientResponse.From()` | xUnit + Moq |
| Integration | Full HTTP pipeline — routing, serialisation, status codes | xUnit + WebApplicationFactory |

### Running the Tests

Run all tests:

```bash
dotnet test
```

Run only unit tests or integration tests by filtering on the namespace:

```bash
dotnet test --filter "FullyQualifiedName~Unit"
dotnet test --filter "FullyQualifiedName~Integration"
```

### What the Integration Tests Cover

- `GET /patients/{id}` with a valid ID returns `200` with a correctly shaped `PatientResponse`
- `dateOfBirth` is serialised as a date-only string (e.g. `"1985-03-12"`) — proving the `DateOnly` shaping works end-to-end through JSON serialisation, not just at the mapping layer
- `GET /patients/{id}` with an unknown ID returns `404` with an `ErrorResponse` body containing the patient ID
- `GET /patients/abc` with a non-integer ID returns `404` — the `:int` route constraint means the route does not match at all rather than returning a `400`

## Continuous Integration

A GitHub Actions workflow runs on every push to `dev` and on every pull request targeting `main`.

**Why these two triggers:**

- **Push to `dev`** — gives fast feedback during active development. Test failures are caught and fixed on the feature branch before they have a chance to accumulate or block others.
- **Pull request to `main`** — acts as a hard quality gate before any code reaches the stable branch. The workflow must pass before a merge is permitted, making it impossible to introduce a regression into `main` undetected.

Merging to `main` is intentionally not a separate trigger — if the PR check passed, running the same suite again on merge would be redundant and waste CI time.

To enforce the PR check as a required status check, go to **Settings → Branches → Add rule for `main`** and enable **Require status checks to pass before merging**, selecting the `Build and Test` check. This makes the gate enforceable at the repository level rather than relying on convention.

## Tech Stack

- **.NET 10**
- **ASP.NET Core Minimal APIs**
- **Microsoft.AspNetCore.OpenApi**
- **Scalar** (interactive API reference UI)
- **xUnit** + **Moq** (testing)
