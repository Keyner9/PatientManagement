# Patient Management Frontend

## Description

Patient Management Frontend is a modern, single-page application for managing patient records. Built with Angular 22 and PrimeNG, it provides a complete CRUD interface with server-side pagination, filtering, CSV export, and a responsive design. The application follows a feature-based architecture with standalone components and lazy-loaded routes.

## Technologies

- **Angular 22** — Latest Angular version with standalone component architecture
- **TypeScript** — Typed superset of JavaScript for robust development
- **PrimeNG 22 RC** — UI component library with Aura design theme
- **RxJS** — Reactive programming for async data streams and state management
- **Angular Router** — Client-side routing with lazy-loaded feature modules
- **Angular HttpClient** — HTTP communication with the backend API
- **Angular Reactive Forms** — Type-safe, reactive form validation and control
- **PrimeIcons** — Icon set used throughout the UI

## Architecture

The application follows a feature-based, layered architecture within the Angular standalone component paradigm:

- **Standalone Components** — All components are standalone with explicit imports, eliminating NgModules entirely
- **Lazy Loading** — Patient feature routes are lazy-loaded for optimal bundle size
- **Services** — Singleton services (`providedIn: 'root'`) encapsulate all HTTP communication and business logic
- **Models** — TypeScript interfaces define strict contracts for all DTOs and API responses
- **Interceptor** — A functional HTTP interceptor handles global error management, displaying toast messages for 400, 404, 409, 500, and connection errors
- **Layout** — A shared layout component provides the app shell with toolbar, toast notifications, and content area
- **Feature-based Structure** — Code is organized by feature (`features/patients/`) with pages, components, and routes scoped to each domain

### Layer Overview

```
src/app/
├── core/                  # Singleton services, layout, interceptors
│   ├── interceptors/      # HTTP error handler interceptor
│   └── layout/            # App shell with toolbar + toast
├── features/              # Feature modules (lazy-loaded)
│   └── patients/          # Patient CRUD feature
│       ├── pages/         # Page components (list, form, detail)
│       └── patients.routes.ts
├── models/                # TypeScript interfaces for all DTOs
├── services/              # HTTP service layer
└── shared/                # Shared reusable components (future)
```

## Requirements

- **Node.js** — v18 or later (v22.18.0 recommended)
- **npm** — v9 or later (v11.16.0 included)
- **Angular CLI** — v22 (installed as dev dependency, run via `npm run ng`)

## Installation

```bash
npm install
```

## Configuration

The backend API URL is configured per environment:

- **`src/app/environments/environment.development.ts`** — Development: `http://localhost:5141/api`
- **`src/app/environments/environment.ts`** — Production: `https://localhost:7158/api`

The API requires a running instance of the PatientManagement.WebApi project with CORS enabled for `http://localhost:4200`.

## Execution

```bash
npm start
```

Launches the development server at `http://localhost:4200` with hot-reload.

## Build

```bash
npm run build
```

Produces an optimized production build in the `dist/` directory. The build enforces size budgets (800kB warning, 1MB error for initial bundle).

## Testing

```bash
npm test
```

Unit tests are executed with Vitest (configured via `@angular/build:unit-test` builder). Tests follow the Jasmine-style syntax and cover services, components, and error handling.

## Features

- **Patient CRUD** — Create, read, update, and delete patient records via a clean REST API
- **Server-side Pagination** — Paginated patient list with configurable page size
- **Search & Filtering** — Debounced search by name and document number
- **Patient Detail View** — Read-only card displaying all patient information
- **Edit Mode** — Reusable form component for both creating and editing patients
- **Delete with Confirmation** — Confirmation dialog before deleting a patient
- **CSV Export** — One-click export of the current patient list to CSV
- **Duplicate Detection** — Inline error handling for duplicate document numbers (HTTP 409)
- **Global Error Handling** — Unified toast notifications for all HTTP errors via interceptor
- **Loading States** — Skeleton placeholders during data loading for perceived performance
- **Empty State** — Professional empty state with icon and helpful message when no results match
- **Responsive Design** — Mobile-first responsive layout with adaptive filter and table rendering
- **Accessibility** — ARIA labels, titles, proper autocomplete attributes, and keyboard navigation support

## Technical Decisions

### OnPush Change Detection

All components use `ChangeDetectionStrategy.OnPush` to minimize change detection cycles. This improves runtime performance by only checking components when their inputs change or when explicitly triggered via signals.

### inject() Dependency Injection

The `inject()` function is used instead of constructor-based DI for cleaner, more readable component code and better tree-shakeability.

### takeUntilDestroyed()

RxJS subscriptions are managed with the `takeUntilDestroyed()` operator, which automatically unsubscribes when the component is destroyed — eliminating manual cleanup and preventing memory leaks.

### PrimeNG

PrimeNG was chosen as the UI component library for its comprehensive set of high-quality components (table, paginator, date picker, confirm dialog, toast, skeleton) and its seamless integration with Angular's standalone component architecture. The Aura theme provides a modern, clean design system.

### Reactive Forms

Angular Reactive Forms are used for type-safe form validation and control. They provide a predictable, testable approach to form management with built-in validators and custom error handling for server-side validation (e.g., duplicate document detection).

### Signals

Angular signals are used selectively where they provide clear value, such as for `loading`, `pageSize`, and filter value state. Combined with RxJS for async data streams, this hybrid approach avoids over-engineering while maintaining reactivity.

## Folder Structure

```
PatientManagement.Frontend/
├── public/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── interceptors/
│   │   │   └── layout/
│   │   ├── features/
│   │   │   └── patients/
│   │   │       ├── pages/
│   │   │       │   ├── patient-list/
│   │   │       │   ├── patient-form/
│   │   │       │   └── patient-detail/
│   │   │       └── patients.routes.ts
│   │   ├── models/
│   │   ├── services/
│   │   └── shared/
│   ├── environments/
│   ├── index.html
│   ├── main.ts
│   └── styles.css
├── angular.json
├── package.json
└── tsconfig.json
```
