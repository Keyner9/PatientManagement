# Patient Management

Full-stack patient management application with a **.NET 9** backend following **Clean Architecture** and an **Angular 22** frontend using **PrimeNG**.

> **Español:** [README.es.md](README.es.md)

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Project Highlights](#project-highlights)
3. [Clean Architecture](#clean-architecture)
4. [Technology Stack](#technology-stack)
5. [Solution Structure](#solution-structure)
6. [Request Flow Diagram](#request-flow-diagram)
7. [Implemented Features](#implemented-features)
8. [REST API Endpoints](#rest-api-endpoints)
9. [Database](#database)
10. [SQL Scripts](#sql-scripts)
11. [Entity Framework Migrations](#entity-framework-migrations)
12. [Validation](#validation)
13. [Exception Handling](#exception-handling)
14. [Testing](#testing)
15. [Running the Project](#running-the-project)
16. [Design Decisions](#design-decisions)
17. [Future Improvements](#future-improvements)

---

## Project Overview

**Patient Management** is a full-stack application that provides a complete CRUD interface for managing patient records. It consists of:

- **Backend**: A .NET 9 RESTful API following Clean Architecture with 46 unit tests
- **Frontend**: An Angular 22 SPA with PrimeNG components, lazy loading, and responsive design
- **Database**: SQL Server with 11 SQL scripts covering tables, indexes, stored procedures, functions, and example queries

The application stores patient information including personal identification (document type and number), full name, birth date, and optional contact details (phone and email). It enforces data integrity through a unique constraint on document type and number, ensuring no duplicate patient records.

The backend follows Clean Architecture with four layers — Domain, Application, Infrastructure, and API — and includes 46 unit tests. The frontend uses standalone components, OnPush change detection, and a feature-based architecture with lazy-loaded routes.

---

## Project Highlights

- **Clean Architecture**: Strict layering with no circular dependencies. Domain knows nothing of other layers.
- **CQRS-like separation**: Distinct DTOs for creation (`CreatePatientDto`), update (`UpdatePatientDto`), detail view (`PatientDto`), and list view (`PatientListDto`).
- **FluentValidation**: Automatic request validation with `AddFluentValidationAutoValidation()`, returning 400 Bad Request for invalid payloads.
- **Standardized API responses**: Every endpoint returns `ApiResponse<T>` with `success`, `message`, and `data` fields.
- **Global exception handling**: `ExceptionMiddleware` maps domain exceptions to proper HTTP status codes (404, 409, 500).
- **Duplicate detection**: Unique database index + application-level check for `(DocumentType, DocumentNumber)`.
- **Pagination**: `GET /api/patients` supports paginated results with `Page` and `PageSize` query parameters.
- **Filtering**: Supports filtering by name (partial match) and document number (exact match).
- **Test coverage**: 46 unit tests using xUnit + Moq, with no dependency on a real database.

---

## Clean Architecture

```
┌──────────────────────────────────────────────────┐
│              PatientManagement.API               │
│     (Controllers, Middleware, Program.cs)        │
├──────────────────────────────────────────────────┤
│           PatientManagement.Application          │
│  (Services, DTOs, Validators, Mapping,           │
│   Interfaces, Exceptions, Common)                │
├──────────────────────────────────────────────────┤
│          PatientManagement.Infrastructure         │
│  (DbContext, Repositories, Migrations,           │
│   DependencyInjection)                           │
├──────────────────────────────────────────────────┤
│             PatientManagement.Domain             │
│  (Entities)                                      │
└──────────────────────────────────────────────────┘
```

### Layer Rules

| Layer | Dependencies | Responsibility |
|---|---|---|
| **Domain** | None | Enterprise business entities (`Patient`) |
| **Application** | Domain | Business logic, validation, DTOs, AutoMapper profiles, service/repository interfaces, custom exceptions, unified response model |
| **Infrastructure** | Application, Domain | EF Core DbContext, repository implementations, migrations, DI registration |
| **API** | Application, Infrastructure | Controllers, middleware (exception handling, CORS), configuration (Swagger, CORS) |

**Strict rules:**
- Controllers contain **zero business logic**. They delegate entirely to `IPatientService`.
- The service layer throws custom exceptions (`NotFoundException`, `DuplicatePatientException`) that the middleware catches and translates to HTTP responses.
- Infrastructure depends on Application (interfaces) and Domain (entities), never the other way around.
- The API layer never references Infrastructure directly except through DI registration.

---

## Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET SDK | 9.0 | Runtime and build toolchain |
| ASP.NET Core | 9.0 | Web framework |
| Entity Framework Core | 9.0.15 | ORM for data access |
| SQL Server | — | Relational database |
| AutoMapper | 16.1.1 | Object-to-object mapping |
| FluentValidation | 12.1.1 | Validation rules library |
| FluentValidation.AspNetCore | 11.3.0 | ASP.NET integration for automatic validation |
| FluentValidation.DependencyInjectionExtensions | 11.9.2 | DI registration for validators |
| Swashbuckle.AspNetCore (Swagger) | 6.9.0 | API documentation UI |
| xUnit | 2.9.2 | Unit testing framework |
| Moq | 4.20.72 | Mocking framework for tests |
| coverlet.collector | 6.0.2 | Code coverage collector |

---

## Solution Structure

```
PatientManagement/
├── PatientManagement.slnx
├── README.md
├── README.es.md
├── sql/
│   ├── 001_CreateDatabase.sql
│   ├── 002_CreatePatientsTable.sql
│   ├── 003_CreateIndexes.sql
│   ├── 004_StoredProcedures.sql
│   ├── 005_TestData.sql
│   ├── 006_CreateDoctorsTable.sql          (*)
│   ├── 007_CreateAppointmentsTable.sql     (*)
│   ├── 008_QueryExamples.sql               (*)
│   ├── 009_AdditionalStoredProcedures.sql   (*)
│   ├── 010_Functions.sql                   (*)
│   └── 011_AdditionalIndexes.sql           (*)
├── PatientManagement.Domain/
│   └── ...
├── PatientManagement.Application/
│   └── ...
├── PatientManagement.Infrastructure/
│   └── ...
├── PatientManagement.API/
│   └── ...
├── PatientManagement.Tests/
│   └── ...
└── PatientManagement.Frontend/             (*)
    ├── README.md
    ├── README.es.md                         (*)
    ├── angular.json
    ├── karma.conf.js                        (*)
    ├── package.json
    └── src/
        ├── app/
        │   ├── app.config.ts
        │   ├── app.routes.ts
        │   ├── core/
        │   │   ├── interceptors/
        │   │   │   └── error-handler.interceptor.ts
        │   │   └── layout/
        │   │       └── layout.component.ts
        │   ├── features/
        │   │   └── patients/
        │   │       ├── patients.routes.ts
        │   │       └── pages/
        │   │           ├── patient-list/
        │   │           ├── patient-form/
        │   │           └── patient-detail/
        │   ├── models/
        │   │   └── patient.ts
        │   └── services/
        │       └── patient.service.ts
        ├── environments/
        ├── index.html
        ├── main.ts
        └── styles.css

  (*) Added in this phase
```

---

## Request Flow Diagram

```
Client (Angular / curl)
        │
        ▼
  ┌─────────────────┐
  │   Middleware     │  ExceptionMiddleware (catches all unhandled exceptions)
  │  (Exception)     │  - NotFoundException      → 404
  │                  │  - DuplicatePatientException → 409
  │                  │  - Unhandled Exception     → 500
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Controller    │  PatientsController
  │  (Patients)     │  - Receives HTTP request
  │                  │  - Extracts route/query/body parameters
  │                  │  - Delegates to IPatientService
  │                  │  - Returns IActionResult
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │    Service      │  PatientService (Application layer)
  │  (Patient)      │  - Validates business rules (duplicate check)
  │                  │  - Maps DTOs ↔ Entities via AutoMapper
  │                  │  - Calls IPatientRepository
  │                  │  - Returns ApiResponse<T>
  │                  │  - Throws custom exceptions on error
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Repository    │  PatientRepository (Infrastructure)
  │  (Patient)      │  - Executes EF Core queries
  │                  │  - AsNoTracking for reads
  │                  │  - Returns domain entities
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Database      │  SQL Server (PatientManagementDb)
  │  (SQL Server)   │  - Patients table
  │                  │  - Unique index on (DocumentType, DocumentNumber)
  │                  │  - Stored procedure: usp_GetPatientsCreatedAfterDate
  └─────────────────┘
```

### Validation Flow (before service)

```
HTTP Request
    │
    ▼
FluentValidation Auto Validation (AOP)
    │
    ├── Valid? ──► Proceed to Controller
    │
    └── Invalid? ──► Return 400 Bad Request with validation errors
```

---

## Implemented Features

### CRUD Operations

| Feature | Endpoint | Description |
|---|---|---|
| **List patients** | `GET /api/patients` | Paginated list with optional filters by name and document number |
| **Get patient** | `GET /api/patients/{id}` | Single patient detail by ID |
| **Create patient** | `POST /api/patients` | Register a new patient (rejects duplicates) |
| **Update patient** | `PUT /api/patients/{id}` | Update existing patient data (rejects document conflicts) |
| **Delete patient** | `DELETE /api/patients/{id}` | Remove a patient record |

### Additional Capabilities

- **Pagination**: The `PagedResult<T>` model includes `TotalCount`, `Page`, `PageSize`, `TotalPages`, `HasPreviousPage`, and `HasNextPage`.
- **Filtering by name**: Searches `FirstName` and `LastName` using `Contains` (substring match).
- **Filtering by document number**: Exact match on `DocumentNumber`.
- **Duplicate prevention**: Both at the database level (unique index) and application level (explicit check before insert/update).
- **Unified response format**: Every endpoint returns `ApiResponse<T>` with `success`, `message`, and `data`.
- **CORS**: Configured for `http://localhost:4200` (Angular development server).
- **Swagger**: Interactive API documentation at `/swagger` (Development only).

---

## REST API Endpoints

### Endpoint Summary

| Method | Route | Description | Request | Response | HTTP Codes |
|---|---|---|---|---|---|
| `GET` | `/api/patients` | List all patients (paginated) | Query params | `ApiResponse<PagedResult<PatientListDto>>` | `200` |
| `GET` | `/api/patients/{id}` | Get patient by ID | Route param | `ApiResponse<PatientDto>` | `200`, `404` |
| `POST` | `/api/patients` | Create a new patient | JSON body | `ApiResponse<PatientDto>` | `201`, `400`, `409` |
| `PUT` | `/api/patients/{id}` | Update an existing patient | Route param + JSON body | `ApiResponse<PatientDto>` | `200`, `400`, `404`, `409` |
| `DELETE` | `/api/patients/{id}` | Delete a patient | Route param | `204 No Content` | `204`, `404` |

### Query Parameters for `GET /api/patients`

| Parameter | Type | Description | Default |
|---|---|---|---|
| `Name` | `string` | Filters by first name or last name (substring match) | — |
| `DocumentNumber` | `string` | Filters by document number (exact match) | — |
| `Page` | `int` | Page number | `1` |
| `PageSize` | `int` | Number of items per page | `10` |

### Request Body Schemas

#### `POST /api/patients` — CreatePatientDto

```json
{
  "documentType": "DNI",
  "documentNumber": "87654321",
  "firstName": "Maria",
  "lastName": "Lopez",
  "birthDate": "1990-05-15",
  "phoneNumber": "555-1234",
  "email": "maria@email.com"
}
```

`phoneNumber` and `email` are optional.

#### `PUT /api/patients/{id}` — UpdatePatientDto

Same schema as `CreatePatientDto`.

### Response Format

All endpoints (except DELETE) return the unified `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "Operacion completada correctamente.",
  "data": { }
}
```

Error responses:

```json
{
  "success": false,
  "message": "No se encontro el paciente con id 999.",
  "data": null
}
```

The DELETE endpoint returns `204 No Content` with no body on success.

### Example: Create a Patient

```bash
curl -X POST http://localhost:5000/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "documentType": "DNI",
    "documentNumber": "87654321",
    "firstName": "Maria",
    "lastName": "Lopez",
    "birthDate": "1990-05-15",
    "phoneNumber": "555-1234",
    "email": "maria@email.com"
  }'
```

Response (201 Created):

```json
{
  "success": true,
  "message": "Paciente creado correctamente.",
  "data": {
    "patientId": 11,
    "documentType": "DNI",
    "documentNumber": "87654321",
    "firstName": "Maria",
    "lastName": "Lopez",
    "birthDate": "1990-05-15",
    "phoneNumber": "555-1234",
    "email": "maria@email.com",
    "createdAt": "2026-07-02T12:00:00Z"
  }
}
```

---

## Database

### Entity: `Patient`

Defined in `PatientManagement.Domain.Entities.Patient`:

| Property | Type | Notes |
|---|---|---|
| `PatientId` | `int` | Primary key, auto-increment |
| `DocumentType` | `string` | Required, max 10 chars |
| `DocumentNumber` | `string` | Required, max 20 chars |
| `FirstName` | `string` | Required, max 80 chars |
| `LastName` | `string` | Required, max 80 chars |
| `BirthDate` | `DateOnly` | Required |
| `PhoneNumber` | `string?` | Optional, max 20 chars |
| `Email` | `string?` | Optional, max 120 chars |
| `CreatedAt` | `DateTime` | Required, auto-set to `DateTime.UtcNow` on creation |

### Table: `Patients`

| Column | Type | Constraint |
|---|---|---|
| `PatientId` | `INT` | `PRIMARY KEY`, `IDENTITY(1,1)` |
| `DocumentType` | `NVARCHAR(10)` | `NOT NULL` |
| `DocumentNumber` | `NVARCHAR(20)` | `NOT NULL` |
| `FirstName` | `NVARCHAR(80)` | `NOT NULL` |
| `LastName` | `NVARCHAR(80)` | `NOT NULL` |
| `BirthDate` | `DATE` | `NOT NULL` |
| `PhoneNumber` | `NVARCHAR(20)` | `NULL` |
| `Email` | `NVARCHAR(120)` | `NULL` |
| `CreatedAt` | `DATETIME2` | `NOT NULL`, `DEFAULT GETUTCDATE()` |

### Unique Index

`UIX_Patients_DocumentType_DocumentNumber` — A unique nonclustered index on `(DocumentType, DocumentNumber)` to prevent duplicate patient records at the database level.

### Stored Procedure

`usp_GetPatientsCreatedAfterDate` — Accepts a `@CreatedAfter DATETIME2` parameter and returns all patients created after that timestamp, ordered by `CreatedAt` descending.

---

## SQL Scripts

Located in the `sql/` directory for manual database setup without relying on EF Core migrations:

| Script | Purpose |
|---|---|---|
| `001_CreateDatabase.sql` | Creates the `PatientManagementDb` database |
| `002_CreatePatientsTable.sql` | Creates the `Patients` table with all columns and constraints |
| `003_CreateIndexes.sql` | Creates the unique index `UIX_Patients_DocumentType_DocumentNumber` |
| `004_StoredProcedures.sql` | Creates `usp_GetPatientsCreatedAfterDate` |
| `005_TestData.sql` | Inserts 10 sample patient records for development/testing |
| `006_CreateDoctorsTable.sql` | Creates the `Doctors` table with specialties and contact info + seed data |
| `007_CreateAppointmentsTable.sql` | Creates the `Appointments` table linked to Patients and Doctors + 20 sample appointments |
| `008_QueryExamples.sql` | Five analytical queries: top patients, doctors without appointments, billing by doctor, multi-specialty patients, recent patients |
| `009_AdditionalStoredProcedures.sql` | Adds `usp_GetDoctorAppointmentsSummary`, `usp_GetPatientAppointmentHistory`, `usp_GetMultiSpecialtyPatients` |
| `010_Functions.sql` | Creates `fn_CalculateAge` to compute age from birth date |
| `011_AdditionalIndexes.sql` | Five indexes with documented rationale for optimizing analytical queries |

Execute in numeric order using SQL Server Management Studio, `sqlcmd`, or any SQL client.

---

## Entity Framework Migrations

A single EF Core migration (`20260702030104_InitialCreate`) generates the `Patients` table matching the entity configuration. The migration is located in `PatientManagement.Infrastructure/Migrations/`.

To apply the migration from the command line:

```bash
cd PatientManagement.Infrastructure
dotnet ef database update --startup-project ../PatientManagement.API
```

The `ApplicationDbContextModelSnapshot.cs` file tracks the current model state for future migrations.

---

## Validation

Input validation is handled by **FluentValidation** at two levels:

### 1. Automatic Request Validation (ASP.NET pipeline)

`AddFluentValidationAutoValidation()` in `Program.cs` automatically validates incoming request bodies against their registered validators before the controller action executes. If validation fails, the API returns `400 Bad Request` with the validation error details — the controller action is never invoked.

### 2. Validator Classes

Both `CreatePatientValidator` and `UpdatePatientValidator` apply identical rules:

| Field | Rule | Message |
|---|---|---|
| `DocumentType` | `NotEmpty`, `MaximumLength(10)` | "El tipo de documento es obligatorio." / "no debe exceder 10 caracteres." |
| `DocumentNumber` | `NotEmpty`, `MaximumLength(20)` | "El numero de documento es obligatorio." / "no debe exceder 20 caracteres." |
| `FirstName` | `NotEmpty`, `MaximumLength(80)` | "El nombre es obligatorio." / "no debe exceder 80 caracteres." |
| `LastName` | `NotEmpty`, `MaximumLength(80)` | "El apellido es obligatorio." / "no debe exceder 80 caracteres." |
| `BirthDate` | `NotEmpty` | "La fecha de nacimiento es obligatoria." |
| `PhoneNumber` | `MaximumLength(20)` when not null | "no debe exceder 20 caracteres." |
| `Email` | `MaximumLength(120)`, `EmailAddress()` when not null | "no debe exceder 120 caracteres." / "Formato de correo electronico invalido." |

Validators are registered via `AddValidatorsFromAssemblyContaining<>` in `Program.cs`, using the assembly that contains `PatientProfile` as the scan target.

---

## Exception Handling

The `ExceptionMiddleware` class (in `PatientManagement.API.Middleware`) wraps every request in a `try/catch` block and maps exceptions to HTTP responses:

| Exception | HTTP Status | Log Level | Response Body |
|---|---|---|---|
| `NotFoundException` | `404 Not Found` | Warning | `ApiResponse<object>.Fail(ex.Message)` |
| `DuplicatePatientException` | `409 Conflict` | Warning | `ApiResponse<object>.Fail(ex.Message)` |
| Any unhandled `Exception` | `500 Internal Server Error` | Error | `ApiResponse<object>.Fail("Ocurrio un error inesperado.")` |

All error responses use the same `ApiResponse<T>` format with `success: false` and contain no sensitive information or stack traces.

---

## Testing

### Test Summary

**46 unit tests** using **xUnit** + **Moq**, with no dependency on a real database or external services.

| Test Class | Tests | Coverage |
|---|---|---|
| `PatientServiceTests` | 11 | Service CRUD operations: create (success, duplicate, CreatedAt), get by ID (found, not found), list (pagination), update (success, not found, document conflict), delete (success, not found) |
| `PatientsControllerTests` | 5 | Controller action response types: `OkObjectResult` (GetAll, GetById, Update), `CreatedAtRouteResult` (Create), `NoContentResult` (Delete) |
| `CreatePatientValidatorTests` | 15 | Validation rules: valid payload, empty/max-length for all 5 required fields, phone/email max-length, invalid email format, null optional fields |
| `UpdatePatientValidatorTests` | 15 | Same coverage as CreatePatientValidatorTests |

### Running Tests

From the solution root:

```bash
dotnet test PatientManagement.Tests
```

Or navigate to the test project:

```bash
cd PatientManagement.Tests
dotnet test
```

Both commands produce a summary of passed, failed, and skipped tests.

---

## Frontend (Angular)

The frontend is built with **Angular 22** and **PrimeNG 22 RC**, following a feature-based architecture with standalone components and lazy-loaded routes.

> For full frontend documentation, see [PatientManagement.Frontend/README.md](PatientManagement.Frontend/README.md) (English) and [README.es.md](PatientManagement.Frontend/README.es.md) (Español).

### Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| Angular | 22.0.5 | Web framework (standalone components) |
| PrimeNG | 22.0.0-rc.1 | UI component library (Aura theme) |
| PrimeIcons | 7.0.0 | Icon library |
| RxJS | 7.8.0 | Reactive data streams |
| Karma | 6.4.4 | Test runner |
| Jasmine | 6.3.0 | Unit testing framework |

### Features

- **Patient CRUD** — Create, read, update, and delete patient records
- **Server-side Pagination** — Paginated patient list with configurable page size
- **Search & Filtering** — Debounced search by name and document number
- **Patient Detail View** — Read-only card displaying all patient information
- **Edit Mode** — Reusable form for both creating and editing patients
- **Delete with Confirmation** — Confirmation dialog before deleting
- **CSV Export** — One-click export of the current patient list to CSV
- **Duplicate Detection** — Inline error handling for duplicate document numbers (HTTP 409)
- **Global Error Handling** — Toast notifications for all HTTP errors via interceptor
- **Loading States** — Skeleton placeholders during data loading
- **Empty State** — Professional empty state with icon and helpful message
- **Responsive Design** — Mobile-first responsive layout
- **Accessibility** — ARIA labels, titles, proper autocomplete attributes

### Architecture Highlights

- **Standalone components** — No NgModules, explicit imports
- **Lazy loading** — Patient feature loaded on demand via `patients.routes.ts`
- **OnPush change detection** — Minimal change detection cycles
- **`inject()` DI** — Functional dependency injection pattern
- **`takeUntilDestroyed()`** — Automatic subscription management
- **Signals** — Used selectively for loading, filter state, and derived values
- **HTTP interceptor** — Functional interceptor for global error handling

### Testing

14 unit tests using **Jasmine + Karma**:

| Test Target | Tests | Coverage |
|---|---|---|
| `PatientService` | 5 | getAll, getById, create, update, delete |
| `PatientListComponent` | 4 | Data loading, table rendering, filters, delete |
| `PatientFormComponent` | 4 | Invalid form, valid form, required validations, email format |
| `AppComponent` | 1 | Component creation |

### Running the Frontend

```bash
cd PatientManagement.Frontend
npm install
npm start       # http://localhost:4200
npm run build   # Production build
npm test        # Unit tests (Jasmine + Karma)
```

## Running the Project

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 22+](https://nodejs.org/) (for the frontend)
- SQL Server (LocalDB, Express, Developer Edition, or any edition)
- (Optional) [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/) or any SQL client

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd PatientManagement
```

### Step 2: Set Up the Database

**Option A — Using SQL Scripts (recommended):**

Execute the scripts in `sql/` in numeric order using your preferred SQL client:

```bash
sqlcmd -S localhost -i sql\001_CreateDatabase.sql
sqlcmd -S localhost -i sql\002_CreatePatientsTable.sql
sqlcmd -S localhost -i sql\003_CreateIndexes.sql
sqlcmd -S localhost -i sql\004_StoredProcedures.sql
sqlcmd -S localhost -i sql\005_TestData.sql
sqlcmd -S localhost -i sql\006_CreateDoctorsTable.sql
sqlcmd -S localhost -i sql\007_CreateAppointmentsTable.sql
sqlcmd -S localhost -i sql\008_QueryExamples.sql
sqlcmd -S localhost -i sql\009_AdditionalStoredProcedures.sql
sqlcmd -S localhost -i sql\010_Functions.sql
sqlcmd -S localhost -i sql\011_AdditionalIndexes.sql
```

**Option B — Using EF Core Migrations:**

```bash
cd PatientManagement.Infrastructure
dotnet ef database update --startup-project ../PatientManagement.API
```

### Step 3: Configure the Connection String

Edit `PatientManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PatientManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Adjust `Server` to match your SQL Server instance (e.g., `(LocalDb)\MSSQLLocalDB`, `localhost\SQLEXPRESS`, or a named instance).

### Step 4: Run the API

```bash
cd PatientManagement.API
dotnet run
```

The API will be available at:
- `http://localhost:5000` (default HTTP)
- `http://localhost:5000/swagger` (Swagger UI, only in Development environment)

### Common Commands

```bash
# Build all projects
dotnet build

# Run all tests
dotnet test PatientManagement.Tests

# Check for outdated packages
dotnet list package --outdated
```

---

## Design Decisions

### 1. CreatedAtRoute Instead of CreatedAtAction

`CreatedAtAction` threw `InvalidOperationException: No route matches the supplied values`. Switching to `CreatedAtRoute("GetPatientById", ...)` with an explicit `Name = "GetPatientById"` on the `[HttpGet("{id:int}")]` attribute resolved the issue.

### 2. AutoMapper 16 Compatibility

AutoMapper version 16.1.1 removed the optional `Action<IMappingOperationOptions>` parameter from `IMapper.Map<T>()`. All Moq mock setups in unit tests were adjusted to exclude this parameter, preventing `ArgumentException` at test runtime.

### 3. FluentValidation Version Alignment

Two FluentValidation packages are used with different versions:
- `FluentValidation` 12.1.1 in the **Application** project for direct validation usage.
- `FluentValidation.AspNetCore` 11.3.0 + `FluentValidation.DependencyInjectionExtensions` **11.9.2** in the **API** project.

The `DependencyInjectionExtensions` package was deliberately pinned to 11.9.2 because version 12.x is incompatible with the `FluentValidation.AspNetCore` 11.x integration, causing a `TypeLoadException` at runtime.

### 4. Test Project as Microsoft.NET.Sdk.Web

The test project (`PatientManagement.Tests.csproj`) uses `Microsoft.NET.Sdk.Web` instead of the default `Microsoft.NET.Sdk`. This is necessary because the controller tests reference ASP.NET Core types such as `OkObjectResult`, `CreatedAtRouteResult`, and `NoContentResult`, which are only available in the Web SDK.

### 5. AsNoTracking for Read Queries

All read operations (`GetByIdAsync`, `GetAllAsync`) use `AsNoTracking()` to improve performance by avoiding the change tracker overhead. Write operations explicitly attach entities via `Update()` or `Add()` before calling `SaveChangesAsync()`.

### 6. Standardized API Response Envelope

Every endpoint (except DELETE which returns `204 No Content`) returns `ApiResponse<T>`. This provides a consistent contract for the Angular frontend: the client always checks `success` before accessing `data`, and `message` provides human-readable context.

### 7. Application-Level Duplicate Check

In addition to the database unique index, the service layer checks for duplicate `(DocumentType, DocumentNumber)` before insert and on document change during update. This provides a better user experience (custom error message) than catching a database constraint violation exception.

### 8. CORS for Angular Development

CORS is configured to allow requests only from `http://localhost:4200`, the default Angular development server address, with any HTTP method and header.

---

## Future Improvements

### Short Term

- **Pagination response headers**: Add `X-Total-Count`, `X-Page`, `X-Page-Size` HTTP headers to `GET /api/patients` for easier client-side handling.
- **Search by CreatedAt range**: Add `CreatedAfter` and `CreatedBefore` filter parameters.
- **Sorting**: Add `SortBy` and `SortOrder` query parameters for `GET /api/patients`.
- **API versioning**: Introduce URL or header-based API versioning (`/api/v1/patients`).

### Medium Term

- **Authentication & Authorization**: Add JWT bearer token authentication with role-based access control.
- **Audit logging**: Implement an audit trail for all create/update/delete operations.
- **Health check endpoint**: Add `GET /api/health` for monitoring and load balancer probes.
- **Serilog**: Replace the default `ILogger` with Serilog for structured logging to file/database.

### Long Term

- **Integration tests**: Add tests that spin up a test container (e.g., Testcontainers for SQL Server) to verify the full stack.
- **Docker support**: Provide a `Dockerfile` and `docker-compose.yml` for containerized deployment.
- **CI/CD pipeline**: Add GitHub Actions workflow for build, test, and deploy.
- **Soft delete**: Replace physical deletion with an `IsActive` flag for data retention.
- **Repository pattern abstraction**: Add a generic `IRepository<T>` base interface for common CRUD operations.
