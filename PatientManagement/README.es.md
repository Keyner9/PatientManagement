# Patient Management

Aplicación full-stack para la gestión de pacientes con un backend **.NET 9** siguiendo **Clean Architecture** y un frontend **Angular 22** con **PrimeNG**.

> **English:** [README.md](README.md)

---

## Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Aspectos Destacados](#aspectos-destacados)
3. [Arquitectura Limpia](#arquitectura-limpia)
4. [Tecnologías](#tecnologías)
5. [Estructura de la Solución](#estructura-de-la-solución)
6. [Diagrama de Flujo de Solicitudes](#diagrama-de-flujo-de-solicitudes)
7. [Funcionalidades Implementadas](#funcionalidades-implementadas)
8. [Endpoints de la API REST](#endpoints-de-la-api-rest)
9. [Base de Datos](#base-de-datos)
10. [Scripts SQL](#scripts-sql)
11. [Migraciones de Entity Framework](#migraciones-de-entity-framework)
12. [Validación](#validación)
13. [Manejo de Excepciones](#manejo-de-excepciones)
14. [Pruebas](#pruebas)
15. [Ejecución del Proyecto](#ejecución-del-proyecto)
16. [Decisiones de Diseño](#decisiones-de-diseño)
17. [Mejoras Futuras](#mejoras-futuras)

---

## Descripción General

**Patient Management** es una aplicación full-stack que proporciona una interfaz CRUD completa para la gestión de registros de pacientes. Está compuesta por:

- **Backend**: Una API RESTful .NET 9 siguiendo Clean Architecture con 46 pruebas unitarias
- **Frontend**: Una SPA Angular 22 con componentes PrimeNG, carga diferida y diseño responsive
- **Base de Datos**: SQL Server con 11 scripts SQL que cubren tablas, índices, procedimientos almacenados, funciones y consultas de ejemplo

La aplicación almacena información de pacientes, incluyendo identificación personal (tipo y número de documento), nombre completo, fecha de nacimiento y datos de contacto opcionales (teléfono y correo electrónico). Garantiza la integridad de los datos mediante una restricción única sobre el tipo y número de documento, evitando registros duplicados.

El backend sigue la Arquitectura Limpia con cuatro capas — Domain, Application, Infrastructure y API — e incluye 46 pruebas unitarias. El frontend utiliza componentes standalone, detección de cambios OnPush, y una arquitectura basada en funcionalidades con rutas de carga diferida.

---

## Aspectos Destacados

- **Arquitectura Limpia**: Capas estrictas sin dependencias circulares. Domain no conoce ninguna otra capa.
- **Separación tipo CQRS**: DTOs distintos para creación (`CreatePatientDto`), actualización (`UpdatePatientDto`), vista de detalle (`PatientDto`) y vista de lista (`PatientListDto`).
- **FluentValidation**: Validación automática de solicitudes con `AddFluentValidationAutoValidation()`, retornando `400 Bad Request` para payloads inválidos.
- **Respuestas API estandarizadas**: Cada endpoint retorna `ApiResponse<T>` con los campos `success`, `message` y `data`.
- **Manejo global de excepciones**: `ExceptionMiddleware` mapea excepciones de dominio a códigos HTTP apropiados (404, 409, 500).
- **Detección de duplicados**: Índice único en base de datos + verificación a nivel de aplicación para `(DocumentType, DocumentNumber)`.
- **Paginación**: `GET /api/patients` soporta resultados paginados con los parámetros `Page` y `PageSize`.
- **Filtros**: Soporta filtrado por nombre (coincidencia parcial) y número de documento (coincidencia exacta).
- **Cobertura de pruebas**: 46 pruebas unitarias con xUnit + Moq, sin dependencia de base de datos real.

---

## Arquitectura Limpia

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

### Reglas por Capa

| Capa | Dependencias | Responsabilidad |
|---|---|---|
| **Domain** | Ninguna | Entidades del negocio (`Patient`) |
| **Application** | Domain | Lógica de negocio, validación, DTOs, perfiles de AutoMapper, interfaces de servicio/repositorio, excepciones personalizadas, modelo de respuesta unificada |
| **Infrastructure** | Application, Domain | DbContext de EF Core, implementaciones de repositorio, migraciones, registro de DI |
| **API** | Application, Infrastructure | Controladores, middleware (manejo de excepciones, CORS), configuración (Swagger, CORS) |

**Reglas estrictas:**
- Los controladores contienen **cero lógica de negocio**. Delegan completamente en `IPatientService`.
- La capa de servicios lanza excepciones personalizadas (`NotFoundException`, `DuplicatePatientException`) que el middleware captura y traduce a respuestas HTTP.
- Infrastructure depende de Application (interfaces) y Domain (entidades), nunca al revés.
- La capa API nunca referencia Infrastructure directamente excepto a través del registro de DI.

---

## Tecnologías

| Tecnología | Versión | Propósito |
|---|---|---|
| .NET SDK | 9.0 | Runtime y herramienta de compilación |
| ASP.NET Core | 9.0 | Framework web |
| Entity Framework Core | 9.0.15 | ORM para acceso a datos |
| SQL Server | — | Base de datos relacional |
| AutoMapper | 16.1.1 | Mapeo objeto a objeto |
| FluentValidation | 12.1.1 | Librería de reglas de validación |
| FluentValidation.AspNetCore | 11.3.0 | Integración ASP.NET para validación automática |
| FluentValidation.DependencyInjectionExtensions | 11.9.2 | Registro DI para validadores |
| Swashbuckle.AspNetCore (Swagger) | 6.9.0 | UI de documentación de API |
| xUnit | 2.9.2 | Framework de pruebas unitarias |
| Moq | 4.20.72 | Framework de mocking para pruebas |
| coverlet.collector | 6.0.2 | Colector de cobertura de código |

---

## Estructura de la Solución

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

  (*) Agregado en esta fase
```

---

## Diagrama de Flujo de Solicitudes

```
Cliente (Angular / curl)
        │
        ▼
  ┌─────────────────┐
  │   Middleware     │  ExceptionMiddleware (captura todas las excepciones no controladas)
  │  (Excepciones)   │  - NotFoundException       → 404
  │                  │  - DuplicatePatientException → 409
  │                  │  - Excepción no controlada  → 500
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Controller    │  PatientsController
  │  (Patients)     │  - Recibe la solicitud HTTP
  │                  │  - Extrae parámetros de ruta/consulta/cuerpo
  │                  │  - Delega en IPatientService
  │                  │  - Retorna IActionResult
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │    Service      │  PatientService (capa Application)
  │  (Patient)      │  - Valida reglas de negocio (verificación de duplicados)
  │                  │  - Mapea DTOs ↔ Entidades via AutoMapper
  │                  │  - Llama a IPatientRepository
  │                  │  - Retorna ApiResponse<T>
  │                  │  - Lanza excepciones personalizadas en error
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Repository    │  PatientRepository (Infrastructure)
  │  (Patient)      │  - Ejecuta consultas EF Core
  │                  │  - AsNoTracking para lecturas
  │                  │  - Retorna entidades de dominio
  └────────┬────────┘
           │
           ▼
  ┌─────────────────┐
  │   Database      │  SQL Server (PatientManagementDb)
  │  (SQL Server)   │  - Tabla Patients
  │                  │  - Índice único en (DocumentType, DocumentNumber)
  │                  │  - Procedimiento almacenado: usp_GetPatientsCreatedAfterDate
  └─────────────────┘
```

### Flujo de Validación (antes del servicio)

```
Solicitud HTTP
    │
    ▼
Validación automática FluentValidation (AOP)
    │
    ├── ¿Válido? ──► Continúa al Controller
    │
    └── ¿Inválido? ──► Retorna 400 Bad Request con errores de validación
```

---

## Funcionalidades Implementadas

### Operaciones CRUD

| Funcionalidad | Endpoint | Descripción |
|---|---|---|
| **Listar pacientes** | `GET /api/patients` | Lista paginada con filtros opcionales por nombre y número de documento |
| **Obtener paciente** | `GET /api/patients/{id}` | Detalle de un paciente por ID |
| **Crear paciente** | `POST /api/patients` | Registrar un nuevo paciente (rechaza duplicados) |
| **Actualizar paciente** | `PUT /api/patients/{id}` | Actualizar datos de un paciente existente (rechaza conflictos de documento) |
| **Eliminar paciente** | `DELETE /api/patients/{id}` | Eliminar un registro de paciente |

### Capacidades Adicionales

- **Paginación**: El modelo `PagedResult<T>` incluye `TotalCount`, `Page`, `PageSize`, `TotalPages`, `HasPreviousPage` y `HasNextPage`.
- **Filtrado por nombre**: Busca en `FirstName` y `LastName` usando `Contains` (coincidencia de subcadena).
- **Filtrado por número de documento**: Coincidencia exacta en `DocumentNumber`.
- **Prevención de duplicados**: Tanto a nivel de base de datos (índice único) como a nivel de aplicación (verificación explícita antes de insertar/actualizar).
- **Formato de respuesta unificado**: Cada endpoint retorna `ApiResponse<T>` con `success`, `message` y `data`.
- **CORS**: Configurado para `http://localhost:4200` (servidor de desarrollo Angular).
- **Swagger**: Documentación interactiva de la API en `/swagger` (solo en Development).

---

## Endpoints de la API REST

### Resumen de Endpoints

| Método | Ruta | Descripción | Solicitud | Respuesta | Códigos HTTP |
|---|---|---|---|---|---|
| `GET` | `/api/patients` | Listar pacientes (paginado) | Parámetros de consulta | `ApiResponse<PagedResult<PatientListDto>>` | `200` |
| `GET` | `/api/patients/{id}` | Obtener paciente por ID | Parámetro de ruta | `ApiResponse<PatientDto>` | `200`, `404` |
| `POST` | `/api/patients` | Crear un nuevo paciente | Cuerpo JSON | `ApiResponse<PatientDto>` | `201`, `400`, `409` |
| `PUT` | `/api/patients/{id}` | Actualizar un paciente existente | Parámetro de ruta + cuerpo JSON | `ApiResponse<PatientDto>` | `200`, `400`, `404`, `409` |
| `DELETE` | `/api/patients/{id}` | Eliminar un paciente | Parámetro de ruta | `204 No Content` | `204`, `404` |

### Parámetros de Consulta para `GET /api/patients`

| Parámetro | Tipo | Descripción | Valor por defecto |
|---|---|---|---|
| `Name` | `string` | Filtra por nombre o apellido (coincidencia parcial) | — |
| `DocumentNumber` | `string` | Filtra por número de documento (coincidencia exacta) | — |
| `Page` | `int` | Número de página | `1` |
| `PageSize` | `int` | Cantidad de elementos por página | `10` |

### Esquemas del Cuerpo de Solicitud

#### `POST /api/patients` — CreatePatientDto

```json
{
  "documentType": "DNI",
  "documentNumber": "87654321",
  "firstName": "María",
  "lastName": "López",
  "birthDate": "1990-05-15",
  "phoneNumber": "555-1234",
  "email": "maria@email.com"
}
```

`phoneNumber` y `email` son opcionales.

#### `PUT /api/patients/{id}` — UpdatePatientDto

Mismo esquema que `CreatePatientDto`.

### Formato de Respuesta

Todos los endpoints (excepto DELETE) retornan el envoltorio unificado `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { }
}
```

Respuestas de error:

```json
{
  "success": false,
  "message": "Patient with id 999 was not found.",
  "data": null
}
```

El endpoint DELETE retorna `204 No Content` sin cuerpo en caso de éxito.

### Ejemplo: Crear un Paciente

```bash
curl -X POST http://localhost:5000/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "documentType": "DNI",
    "documentNumber": "87654321",
    "firstName": "María",
    "lastName": "López",
    "birthDate": "1990-05-15",
    "phoneNumber": "555-1234",
    "email": "maria@email.com"
  }'
```

Respuesta (201 Created):

```json
{
  "success": true,
  "message": "Patient created successfully.",
  "data": {
    "patientId": 11,
    "documentType": "DNI",
    "documentNumber": "87654321",
    "firstName": "María",
    "lastName": "López",
    "birthDate": "1990-05-15",
    "phoneNumber": "555-1234",
    "email": "maria@email.com",
    "createdAt": "2026-07-02T12:00:00Z"
  }
}
```

---

## Base de Datos

### Entidad: `Patient`

Definida en `PatientManagement.Domain.Entities.Patient`:

| Propiedad | Tipo | Notas |
|---|---|---|
| `PatientId` | `int` | Clave primaria, auto-incrementable |
| `DocumentType` | `string` | Requerido, máximo 10 caracteres |
| `DocumentNumber` | `string` | Requerido, máximo 20 caracteres |
| `FirstName` | `string` | Requerido, máximo 80 caracteres |
| `LastName` | `string` | Requerido, máximo 80 caracteres |
| `BirthDate` | `DateOnly` | Requerido |
| `PhoneNumber` | `string?` | Opcional, máximo 20 caracteres |
| `Email` | `string?` | Opcional, máximo 120 caracteres |
| `CreatedAt` | `DateTime` | Requerido, se establece automáticamente a `DateTime.UtcNow` al crear |

### Tabla: `Patients`

| Columna | Tipo | Restricción |
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

### Índice Único

`UIX_Patients_DocumentType_DocumentNumber` — Índice único no agrupado en `(DocumentType, DocumentNumber)` para evitar registros duplicados a nivel de base de datos.

### Procedimiento Almacenado

`usp_GetPatientsCreatedAfterDate` — Acepta un parámetro `@CreatedAfter DATETIME2` y retorna todos los pacientes creados después de esa fecha, ordenados por `CreatedAt` descendente.

---

## Scripts SQL

Ubicados en el directorio `sql/` para configuración manual de la base de datos sin depender de migraciones de EF Core:

| Script | Propósito |
|---|---|---|
| `001_CreateDatabase.sql` | Crea la base de datos `PatientManagementDb` |
| `002_CreatePatientsTable.sql` | Crea la tabla `Patients` con todas las columnas y restricciones |
| `003_CreateIndexes.sql` | Crea el índice único `UIX_Patients_DocumentType_DocumentNumber` |
| `004_StoredProcedures.sql` | Crea `usp_GetPatientsCreatedAfterDate` |
| `005_TestData.sql` | Inserta 10 registros de pacientes de ejemplo para desarrollo/pruebas |
| `006_CreateDoctorsTable.sql` | Crea la tabla `Doctors` con especialidades y datos de contacto + datos de ejemplo |
| `007_CreateAppointmentsTable.sql` | Crea la tabla `Appointments` vinculada a Patients y Doctors + 20 citas de ejemplo |
| `008_QueryExamples.sql` | Cinco consultas analíticas: top pacientes, médicos sin citas, facturación, multi-especialidad, pacientes recientes |
| `009_AdditionalStoredProcedures.sql` | Agrega `usp_GetDoctorAppointmentsSummary`, `usp_GetPatientAppointmentHistory`, `usp_GetMultiSpecialtyPatients` |
| `010_Functions.sql` | Crea `fn_CalculateAge` para calcular la edad desde la fecha de nacimiento |
| `011_AdditionalIndexes.sql` | Cinco índices con justificación documentada para optimizar consultas analíticas |

Ejecutar en orden numérico usando SQL Server Management Studio, `sqlcmd` o cualquier cliente SQL.

---

## Migraciones de Entity Framework

Una migración única de EF Core (`20260702030104_InitialCreate`) genera la tabla `Patients` coincidiendo con la configuración de la entidad. La migración se encuentra en `PatientManagement.Infrastructure/Migrations/`.

Para aplicar la migración desde la línea de comandos:

```bash
cd PatientManagement.Infrastructure
dotnet ef database update --startup-project ../PatientManagement.API
```

El archivo `ApplicationDbContextModelSnapshot.cs` mantiene el estado actual del modelo para futuras migraciones.

---

## Validación

La validación de entrada es manejada por **FluentValidation** en dos niveles:

### 1. Validación Automática de Solicitudes (pipeline ASP.NET)

`AddFluentValidationAutoValidation()` en `Program.cs` valida automáticamente los cuerpos de las solicitudes entrantes contra sus validadores registrados antes de que se ejecute la acción del controlador. Si la validación falla, la API retorna `400 Bad Request` con los detalles del error de validación — la acción del controlador nunca se invoca.

### 2. Clases Validadoras

Tanto `CreatePatientValidator` como `UpdatePatientValidator` aplican reglas idénticas:

| Campo | Regla | Mensaje |
|---|---|---|
| `DocumentType` | `NotEmpty`, `MaximumLength(10)` | "Document type is required." / "must not exceed 10 characters." |
| `DocumentNumber` | `NotEmpty`, `MaximumLength(20)` | "Document number is required." / "must not exceed 20 characters." |
| `FirstName` | `NotEmpty`, `MaximumLength(80)` | "First name is required." / "must not exceed 80 characters." |
| `LastName` | `NotEmpty`, `MaximumLength(80)` | "Last name is required." / "must not exceed 80 characters." |
| `BirthDate` | `NotEmpty` | "Birth date is required." |
| `PhoneNumber` | `MaximumLength(20)` cuando no es nulo | "must not exceed 20 characters." |
| `Email` | `MaximumLength(120)`, `EmailAddress()` cuando no es nulo | "must not exceed 120 characters." / "Invalid email format." |

Los validadores se registran via `AddValidatorsFromAssemblyContaining<>` en `Program.cs`, usando el ensamblado que contiene `PatientProfile` como objetivo de escaneo.

---

## Manejo de Excepciones

La clase `ExceptionMiddleware` (en `PatientManagement.API.Middleware`) envuelve cada solicitud en un bloque `try/catch` y mapea las excepciones a respuestas HTTP:

| Excepción | Estado HTTP | Nivel de Log | Cuerpo de Respuesta |
|---|---|---|---|
| `NotFoundException` | `404 Not Found` | Warning | `ApiResponse<object>.Fail(ex.Message)` |
| `DuplicatePatientException` | `409 Conflict` | Warning | `ApiResponse<object>.Fail(ex.Message)` |
| Cualquier `Exception` no controlada | `500 Internal Server Error` | Error | `ApiResponse<object>.Fail("An unexpected error occurred.")` |

Todas las respuestas de error usan el mismo formato `ApiResponse<T>` con `success: false` y no contienen información sensible ni trazas de pila.

---

## Pruebas

### Resumen de Pruebas

**46 pruebas unitarias** usando **xUnit** + **Moq**, sin dependencia de base de datos real ni servicios externos.

| Clase de Prueba | Pruebas | Cobertura |
|---|---|---|
| `PatientServiceTests` | 11 | Operaciones CRUD del servicio: crear (éxito, duplicado, CreatedAt), obtener por ID (encontrado, no encontrado), listar (paginación), actualizar (éxito, no encontrado, conflicto de documento), eliminar (éxito, no encontrado) |
| `PatientsControllerTests` | 5 | Tipos de respuesta de acciones del controlador: `OkObjectResult` (GetAll, GetById, Update), `CreatedAtRouteResult` (Create), `NoContentResult` (Delete) |
| `CreatePatientValidatorTests` | 15 | Reglas de validación: payload válido, vacío/longitud máxima para los 5 campos requeridos, longitud máxima de teléfono/email, formato de email inválido, campos opcionales nulos |
| `UpdatePatientValidatorTests` | 15 | Misma cobertura que CreatePatientValidatorTests |

### Ejecutar Pruebas

Desde la raíz de la solución:

```bash
dotnet test PatientManagement.Tests
```

O navegar al proyecto de pruebas:

```bash
cd PatientManagement.Tests
dotnet test
```

Ambos comandos producen un resumen de pruebas pasadas, fallidas y omitidas.

---

## Frontend (Angular)

El frontend está construido con **Angular 22** y **PrimeNG 22 RC**, siguiendo una arquitectura basada en funcionalidades con componentes standalone y rutas de carga diferida.

> Para documentación completa del frontend, ver [PatientManagement.Frontend/README.md](PatientManagement.Frontend/README.md) (English) y [README.es.md](PatientManagement.Frontend/README.es.md) (Español).

### Tecnologías

| Tecnología | Versión | Propósito |
|---|---|---|
| Angular | 22.0.5 | Framework web (componentes standalone) |
| PrimeNG | 22.0.0-rc.1 | Librería de componentes UI (tema Aura) |
| PrimeIcons | 7.0.0 | Librería de iconos |
| RxJS | 7.8.0 | Flujos de datos reactivos |
| Karma | 6.4.4 | Ejecutor de pruebas |
| Jasmine | 6.3.0 | Framework de pruebas unitarias |

### Funcionalidades

- **CRUD de Pacientes** — Crear, leer, actualizar y eliminar registros de pacientes
- **Paginación del lado del servidor** — Lista paginada con tamaño de página configurable
- **Búsqueda y Filtrado** — Búsqueda con debounce por nombre y número de documento
- **Vista de Detalle** — Tarjeta de solo lectura con toda la información del paciente
- **Modo Edición** — Formulario reutilizable para crear y editar pacientes
- **Eliminación con Confirmación** — Diálogo de confirmación antes de eliminar
- **Exportación CSV** — Exportación con un clic de la lista actual a CSV
- **Detección de Duplicados** — Manejo de error HTTP 409 para documentos duplicados
- **Manejo Global de Errores** — Notificaciones Toast para errores HTTP via interceptor
- **Estados de Carga** — Placeholders Skeleton durante la carga de datos
- **Estado Vacío** — Estado vacío profesional con icono y mensaje informativo
- **Diseño Responsive** — Layout adaptable para dispositivos móviles
- **Accesibilidad** — ARIA labels, titles, atributos autocomplete

### Aspectos Destacados de la Arquitectura

- **Componentes standalone** — Sin NgModules, imports explícitos
- **Carga diferida** — Funcionalidad de pacientes cargada bajo demanda
- **OnPush change detection** — Ciclos de detección de cambios mínimos
- **`inject()` DI** — Patrón de inyección de dependencias funcional
- **`takeUntilDestroyed()`** — Gestión automática de suscripciones
- **Signals** — Usados selectivamente para estados de carga, filtros y valores derivados
- **Interceptor HTTP** — Interceptor funcional para manejo global de errores

### Pruebas

14 pruebas unitarias usando **Jasmine + Karma**:

| Objetivo de Prueba | Pruebas | Cobertura |
|---|---|---|
| `PatientService` | 5 | getAll, getById, create, update, delete |
| `PatientListComponent` | 4 | Carga de datos, renderizado de tabla, filtros, eliminación |
| `PatientFormComponent` | 4 | Formulario inválido, formulario válido, validaciones requeridas, formato email |
| `AppComponent` | 1 | Creación del componente |

### Ejecutar el Frontend

```bash
cd PatientManagement.Frontend
npm install
npm start       # http://localhost:4200
npm run build   # Compilación de producción
npm test        # Pruebas unitarias (Jasmine + Karma)
```

## Ejecución del Proyecto

### Requisitos Previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 22+](https://nodejs.org/) (para el frontend)
- SQL Server (LocalDB, Express, Developer Edition o cualquier edición)
- (Opcional) [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/) o cualquier cliente SQL

### Paso 1: Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd PatientManagement
```

### Paso 2: Configurar la Base de Datos

**Opción A — Usando Scripts SQL (recomendado):**

Ejecutar los scripts en `sql/` en orden numérico usando su cliente SQL preferido:

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

**Opción B — Usando Migraciones de EF Core:**

```bash
cd PatientManagement.Infrastructure
dotnet ef database update --startup-project ../PatientManagement.API
```

### Paso 3: Configurar la Cadena de Conexión

Editar `PatientManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PatientManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Ajustar `Server` según su instancia de SQL Server (ej. `(LocalDb)\MSSQLLocalDB`, `localhost\SQLEXPRESS`, o una instancia nombrada).

### Paso 4: Ejecutar la API

```bash
cd PatientManagement.API
dotnet run
```

La API estará disponible en:
- `http://localhost:5000` (HTTP por defecto)
- `http://localhost:5000/swagger` (Swagger UI, solo en entorno Development)

### Comandos Comunes

```bash
# Compilar todos los proyectos
dotnet build

# Ejecutar todas las pruebas
dotnet test PatientManagement.Tests

# Verificar paquetes desactualizados
dotnet list package --outdated
```

---

## Decisiones de Diseño

### 1. CreatedAtRoute en Lugar de CreatedAtAction

`CreatedAtAction` lanzaba `InvalidOperationException: No route matches the supplied values`. Cambiar a `CreatedAtRoute("GetPatientById", ...)` con un `Name = "GetPatientById"` explícito en el atributo `[HttpGet("{id:int}")]` resolvió el problema.

### 2. Compatibilidad con AutoMapper 16

AutoMapper versión 16.1.1 eliminó el parámetro opcional `Action<IMappingOperationOptions>` de `IMapper.Map<T>()`. Todas las configuraciones de mocks de Moq en las pruebas unitarias se ajustaron para excluir este parámetro, evitando `ArgumentException` en tiempo de ejecución de las pruebas.

### 3. Alineación de Versiones de FluentValidation

Se utilizan dos paquetes de FluentValidation con diferentes versiones:
- `FluentValidation` 12.1.1 en el proyecto **Application** para uso directo de validación.
- `FluentValidation.AspNetCore` 11.3.0 + `FluentValidation.DependencyInjectionExtensions` **11.9.2** en el proyecto **API**.

El paquete `DependencyInjectionExtensions` se fijó deliberadamente en 11.9.2 porque la versión 12.x es incompatible con la integración de `FluentValidation.AspNetCore` 11.x, causando `TypeLoadException` en tiempo de ejecución.

### 4. Proyecto de Pruebas como Microsoft.NET.Sdk.Web

El proyecto de pruebas (`PatientManagement.Tests.csproj`) usa `Microsoft.NET.Sdk.Web` en lugar del `Microsoft.NET.Sdk` predeterminado. Esto es necesario porque las pruebas del controlador referencian tipos de ASP.NET Core como `OkObjectResult`, `CreatedAtRouteResult` y `NoContentResult`, que solo están disponibles en el Web SDK.

### 5. AsNoTracking para Consultas de Lectura

Todas las operaciones de lectura (`GetByIdAsync`, `GetAllAsync`) usan `AsNoTracking()` para mejorar el rendimiento evitando la sobrecarga del seguimiento de cambios. Las operaciones de escritura adjuntan explícitamente las entidades mediante `Update()` o `Add()` antes de llamar a `SaveChangesAsync()`.

### 6. Envoltorio de Respuesta API Estandarizado

Cada endpoint (excepto DELETE que retorna `204 No Content`) retorna `ApiResponse<T>`. Esto proporciona un contrato consistente para el frontend Angular: el cliente siempre verifica `success` antes de acceder a `data`, y `message` proporciona contexto legible para humanos.

### 7. Verificación de Duplicados a Nivel de Aplicación

Además del índice único en la base de datos, la capa de servicios verifica duplicados de `(DocumentType, DocumentNumber)` antes de insertar y ante cambios de documento durante la actualización. Esto proporciona una mejor experiencia de usuario (mensaje de error personalizado) que capturar una excepción de violación de restricción de base de datos.

### 8. CORS para Desarrollo Angular

CORS está configurado para permitir solicitudes solo desde `http://localhost:4200`, la dirección predeterminada del servidor de desarrollo Angular, con cualquier método y cabecera HTTP.

---

## Mejoras Futuras

### Corto Plazo

- **Cabeceras de paginación en la respuesta**: Agregar cabeceras HTTP `X-Total-Count`, `X-Page`, `X-Page-Size` a `GET /api/patients` para facilitar el manejo del lado del cliente.
- **Búsqueda por rango de CreatedAt**: Agregar parámetros de filtro `CreatedAfter` y `CreatedBefore`.
- **Ordenamiento**: Agregar parámetros de consulta `SortBy` y `SortOrder` para `GET /api/patients`.
- **Versionado de API**: Introducir versionado de API basado en URL o cabeceras (`/api/v1/patients`).

### Mediano Plazo

- **Autenticación y Autorización**: Agregar autenticación JWT con control de acceso basado en roles.
- **Auditoría**: Implementar un registro de auditoría para todas las operaciones de creación/actualización/eliminación.
- **Endpoint de health check**: Agregar `GET /api/health` para monitoreo y sondas de balanceadores de carga.
- **Serilog**: Reemplazar `ILogger` predeterminado con Serilog para registro estructurado en archivo/base de datos.

### Largo Plazo

- **Pruebas de integración**: Agregar pruebas que inicien un contenedor de prueba (ej. Testcontainers para SQL Server) para verificar el stack completo.
- **Soporte Docker**: Proporcionar un `Dockerfile` y `docker-compose.yml` para despliegue contenerizado.
- **Pipeline CI/CD**: Agregar flujo de trabajo de GitHub Actions para compilación, prueba y despliegue.
- **Eliminación lógica (soft delete)**: Reemplazar la eliminación física con una bandera `IsActive` para retención de datos.
- **Abstracción del patrón repositorio**: Agregar una interfaz base genérica `IRepository<T>` para operaciones CRUD comunes.
