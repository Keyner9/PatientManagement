# Patient Management Frontend

## Descripción

Patient Management Frontend es una aplicación moderna de página única para la gestión de registros de pacientes. Construida con Angular 22 y PrimeNG, proporciona una interfaz CRUD completa con paginación del lado del servidor, filtros, exportación CSV y diseño responsive. La aplicación sigue una arquitectura basada en funcionalidades con componentes standalone y rutas de carga diferida.

## Tecnologías

- **Angular 22** — Última versión de Angular con arquitectura de componentes standalone
- **TypeScript** — Superset tipado de JavaScript para desarrollo robusto
- **PrimeNG 22 RC** — Librería de componentes UI con tema de diseño Aura
- **RxJS** — Programación reactiva para flujos de datos asíncronos y gestión de estado
- **Angular Router** — Enrutamiento del lado del cliente con módulos de carga diferida
- **Angular HttpClient** — Comunicación HTTP con la API backend
- **Angular Reactive Forms** — Validación y control de formularios reactivos con tipado seguro
- **PrimeIcons** — Conjunto de iconos utilizado en toda la interfaz

## Arquitectura

La aplicación sigue una arquitectura basada en funcionalidades, con capas dentro del paradigma de componentes standalone de Angular:

- **Componentes Standalone** — Todos los componentes son standalone con imports explícitos, eliminando completamente los NgModules
- **Carga Diferida** — Las rutas de la funcionalidad de pacientes se cargan de forma diferida para optimizar el tamaño del bundle
- **Servicios** — Servicios singleton (`providedIn: 'root'`) encapsulan toda la comunicación HTTP y la lógica de negocio
- **Modelos** — Interfaces TypeScript definen contratos estrictos para todos los DTOs y respuestas de API
- **Interceptor** — Un interceptor HTTP funcional maneja la gestión global de errores, mostrando mensajes Toast para errores 400, 404, 409, 500 y de conexión
- **Layout** — Un componente de layout compartido proporciona la estructura de la aplicación con barra de herramientas, notificaciones Toast y área de contenido
- **Estructura por Funcionalidades** — El código se organiza por funcionalidad (`features/patients/`) con páginas, componentes y rutas específicas de cada dominio

### Vista de Capas

```
src/app/
├── core/                  # Servicios singleton, layout, interceptors
│   ├── interceptors/      # Interceptor de manejo de errores HTTP
│   └── layout/            # Estructura de la app con toolbar + toast
├── features/              # Módulos de funcionalidad (carga diferida)
│   └── patients/          # Funcionalidad CRUD de pacientes
│       ├── pages/         # Componentes de página (lista, formulario, detalle)
│       └── patients.routes.ts
├── models/                # Interfaces TypeScript para todos los DTOs
├── services/              # Capa de servicios HTTP
└── shared/                # Componentes reutilizables compartidos (futuro)
```

## Requisitos

- **Node.js** — v18 o superior (v22.18.0 recomendado)
- **npm** — v9 o superior (v11.16.0 incluido)
- **Angular CLI** — v22 (instalado como dependencia de desarrollo, ejecutar via `npm run ng`)

## Instalación

```bash
npm install
```

## Configuración

La URL de la API backend se configura por entorno:

- **`src/app/environments/environment.development.ts`** — Desarrollo: `http://localhost:5141/api`
- **`src/app/environments/environment.ts`** — Producción: `https://localhost:7158/api`

La API requiere una instancia en ejecución del proyecto PatientManagement.WebApi con CORS habilitado para `http://localhost:4200`.

## Ejecución

```bash
npm start
```

Inicia el servidor de desarrollo en `http://localhost:4200` con recarga en caliente.

## Build

```bash
npm run build
```

Produce una compilación de producción optimizada en el directorio `dist/`. La compilación aplica presupuestos de tamaño (800kB de advertencia, 1MB de error para el bundle inicial).

## Testing

```bash
npm test
```

Las pruebas unitarias se ejecutan con Jasmine + Karma (configurado via el builder `@angular/build:karma`). Cubren servicios, componentes y manejo de errores.

## Funcionalidades

- **CRUD de Pacientes** — Crear, leer, actualizar y eliminar registros de pacientes via API REST
- **Paginación del Lado del Servidor** — Lista paginada con tamaño de página configurable
- **Búsqueda y Filtrado** — Búsqueda con debounce por nombre y número de documento
- **Vista de Detalle** — Tarjeta de solo lectura que muestra toda la información del paciente
- **Modo Edición** — Componente de formulario reutilizable para crear y editar pacientes
- **Eliminación con Confirmación** — Diálogo de confirmación antes de eliminar un paciente
- **Exportación CSV** — Exportación con un clic de la lista actual de pacientes a CSV
- **Detección de Duplicados** — Manejo de error HTTP 409 para números de documento duplicados
- **Manejo Global de Errores** — Notificaciones Toast unificadas para todos los errores HTTP via interceptor
- **Estados de Carga** — Placeholders Skeleton durante la carga de datos para mejorar la percepción de rendimiento
- **Estado Vacío** — Estado vacío profesional con icono y mensaje informativo cuando no hay resultados
- **Diseño Responsive** — Layout adaptable para móviles con filtros y tabla responsivos
- **Accesibilidad** — ARIA labels, titles, atributos autocomplete adecuados y soporte de navegación por teclado

## Decisiones Técnicas

### OnPush Change Detection

Todos los componentes usan `ChangeDetectionStrategy.OnPush` para minimizar los ciclos de detección de cambios. Esto mejora el rendimiento en tiempo de ejecución al solo verificar componentes cuando sus entradas cambian o cuando se activa explícitamente via signals.

### inject() Dependency Injection

Se utiliza la función `inject()` en lugar de la inyección por constructor para un código de componente más limpio y legible, y mejor capacidad de tree-shaking.

### takeUntilDestroyed()

Las suscripciones RxJS se gestionan con el operador `takeUntilDestroyed()`, que se desuscribe automáticamente cuando el componente se destruye — eliminando la limpieza manual y previniendo fugas de memoria.

### PrimeNG

PrimeNG fue elegido como librería de componentes UI por su conjunto completo de componentes de alta calidad (tabla, paginador, selector de fecha, diálogo de confirmación, toast, skeleton) y su integración perfecta con la arquitectura de componentes standalone de Angular. El tema Aura proporciona un sistema de diseño moderno y limpio.

### Reactive Forms

Se utilizan Reactive Forms de Angular para la validación y control de formularios con tipado seguro. Proporcionan un enfoque predecible y testeable para la gestión de formularios con validadores incorporados y manejo personalizado de errores para validación del lado del servidor (ej. detección de documentos duplicados).

### Signals

Las signals de Angular se utilizan selectivamente donde aportan valor claro, como para el estado de `loading`, `pageSize` y valores de filtro. Combinadas con RxJS para flujos de datos asíncronos, este enfoque híbrido evita la sobreingeniería mientras mantiene la reactividad.

## Estructura de Carpetas

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
├── karma.conf.js
├── package.json
└── tsconfig.json
```
