# VainillaSystem

[![.NET](https://github.com/candekn/VainillaSystem/actions/workflows/dotnet.yml/badge.svg)](https://github.com/candekn/VainillaSystem/actions/workflows/dotnet.yml)

**🇦🇷 Versión en Español** · **[🇬🇧 English Version](#vainillasystem--english-version)**

| 🇦🇷 Español | 🇬🇧 English |
|---|---|
| [Estructura de la solución](#estructura-de-la-solución) | [Solution Structure](#solution-structure) |
| [Arquitectura](#arquitectura) | [Architecture](#architecture-1) |
| [Casos de uso implementados](#casos-de-uso-implementados) | [Implemented Use Cases](#implemented-use-cases) |
| [Conceptos clave](#conceptos-clave) | [Key Concepts](#key-concepts) |
| [Cómo ejecutar](#cómo-ejecutar) | [How to Run](#how-to-run) |
| [Tests unitarios](#tests-unitarios) | [Unit Tests](#unit-tests) |
| [Tecnologías](#tecnologías) | [Technologies](#technologies) |

---

Implementación educativa de un **Mediador Vainilla** con **CQRS** y **Clean Architecture** en .NET 8, sin dependencias externas como MediatR.

> El objetivo del proyecto es demostrar cómo construir un pipeline de mediación completo —con logging, caché e invalidación— usando únicamente el ecosistema base de .NET: `IServiceProvider`, Reflection y el patrón Decorator.

---

## Estructura de la solución

```
VainillaSystem/
├── VainillaSystem.Domain              # Entidades, Value Objects e Interfaces base
├── VainillaSystem.Application         # Commands, Queries, Handlers e IMediator
├── VainillaSystem.Infrastructure      # MyVanillaMediator, Decorators, Repositorio, DI
├── VainillaSystem.Api                 # Controladores y configuración
│
├── VainillaSystem.Domain.UnitTests
├── VainillaSystem.Application.UnitTests
└── VainillaSystem.Infrastructure.UnitTests
```

---

## Arquitectura

### Pipeline de ejecución

Cada request pasa por una cadena de decoradores antes de llegar al handler concreto:

```
UsersController
    └─► MyVanillaMediator
            └─► LoggingBehavior          ← mide y loguea tiempo de ejecución
                    └─► InvalidateCacheBehavior  ← borra keys de caché (solo Commands)
                            └─► CachingBehavior  ← sirve desde caché (solo Queries)
                                    └─► ConcreteHandler
```

> **Principio Open/Closed:** los handlers no saben nada de caché ni logging. Los behaviors se agregan como decoradores sin tocar el handler.

### Capas y responsabilidades

| Capa | Responsabilidad |
|------|----------------|
| **Domain** | Entidades, Value Objects con validación, interfaces `IRequest`, `IRequestHandler`, `ICachableRequest`, `IInvalidateCacheRequest` |
| **Application** | Commands/Queries (records), Handlers, interfaz `IMediator` |
| **Infrastructure** | `MyVanillaMediator` (via Reflection), Behaviors (Decorators), `InMemoryUserRepository`, escaneo automático de ensamblados en DI |
| **Api** | Controladores REST, `Program.cs` |

---

## Casos de uso implementados

### Crear un Usuario — `POST /api/users`

```http
POST /api/users
Content-Type: application/json

{ "name": "Monkey D. Luffy", "age": 19 }
```

```json
// 201 Created
{ "id": "3fa85f64-...", "name": "Monkey D. Luffy", "age": 19 }
```

- Ejecuta `CreateUserCommand` → `CreateUserHandler`
- Crea el agregado `User` con un nuevo `Guid`
- Al finalizar, `InvalidateCacheBehavior` borra las keys `user-{id}` y `users-list`

---

### Obtener un Usuario — `GET /api/users/{id}`

```http
GET /api/users/3fa85f64-...
```

```json
// 200 OK
{ "id": "3fa85f64-...", "name": "Monkey D. Luffy", "age": 19 }
```

- Ejecuta `GetUserByIdQuery` → `GetUserByIdHandler`
- En la primera llamada: **CACHE MISS** → consulta el repositorio y almacena el resultado 5 minutos
- En llamadas posteriores: **CACHE HIT** → respuesta directa desde `IMemoryCache`

---

### Actualizar un Usuario — `PUT /api/users/{id}`

- Ejecuta `UpdateUserCommand` → `UpdateUserHandler`
- Carga el agregado, llama `user.UpdateDetails(name, age)` siguiendo DDD
- `InvalidateCacheBehavior` limpia el caché del usuario automáticamente

---

## Conceptos clave

### MyVanillaMediator

Resuelve el handler adecuado desde `IServiceProvider` usando Reflection y despacha el request:

```csharp
var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
var handler     = _serviceProvider.GetRequiredService(handlerType);
```

### Registro automático (DependencyInjection.cs)

Escanea el ensamblado de Application, encuentra todos los `IRequestHandler<,>` concretos y los envuelve automáticamente con los decoradores, sin ningún registro manual por caso de uso:

```csharp
builder.Services.AddInfrastructure(typeof(CreateUserCommand).Assembly);
```

### Value Objects con validación

```csharp
Age.Create(5);    // ❌ ArgumentException: Age must be between 13 and 99
Name.Create("");  // ❌ ArgumentException: Name cannot be null or empty
```

---

## Cómo ejecutar

### Requisitos
- .NET 8 SDK

### Iniciar la API

```bash
dotnet run --project VainillaSystem.Api --launch-profile http
```

Swagger UI disponible en: `http://localhost:5239/swagger`

### Ejecutar los tests

```bash
dotnet test VainillaSystem.sln --filter "FullyQualifiedName~UnitTests"
```

**Resultado actual:** ✅ 39 tests — 0 fallos

---

## Tests unitarios

| Proyecto | Clases cubiertas |
|----------|-----------------|
| `Domain.UnitTests` | `EntityId`, `Age`, `Name`, `User` |
| `Application.UnitTests` | `CreateUserHandler`, `GetUserByIdHandler` |
| `Infrastructure.UnitTests` | `CachingBehavior`, `InvalidateCacheBehavior`, `LoggingBehavior`, `InMemoryUserRepository` |

Los mocks se hacen con **NSubstitute**.

---

## Tecnologías

- [.NET 8](https://dotnet.microsoft.com/)
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core)
- [xUnit](https://xunit.net/)
- [NSubstitute](https://nsubstitute.github.io/)
- `Microsoft.Extensions.Caching.Memory`
- `Microsoft.Extensions.Logging.Abstractions`

---

<br/>

---

# VainillaSystem — English version

An educational implementation of a **Vanilla Mediator** with **CQRS** and **Clean Architecture** in .NET 8, without external dependencies like MediatR.

> The goal of this project is to demonstrate how to build a complete mediation pipeline —with logging, caching, and cache invalidation— using only the .NET base ecosystem: `IServiceProvider`, Reflection, and the Decorator pattern.

---

## Solution Structure

```
VainillaSystem/
├── VainillaSystem.Domain              # Entities, Value Objects, and base Interfaces
├── VainillaSystem.Application         # Commands, Queries, Handlers, and IMediator
├── VainillaSystem.Infrastructure      # MyVanillaMediator, Decorators, Repository, DI
├── VainillaSystem.Api                 # Controllers and configuration
│
├── VainillaSystem.Domain.UnitTests
├── VainillaSystem.Application.UnitTests
└── VainillaSystem.Infrastructure.UnitTests
```

---

## Architecture

### Execution Pipeline

Each request travels through a chain of decorators before reaching the concrete handler:

```
UsersController
    └─► MyVanillaMediator
            └─► LoggingBehavior          ← measures and logs execution time
                    └─► InvalidateCacheBehavior  ← removes cache keys (Commands only)
                            └─► CachingBehavior  ← serves from cache (Queries only)
                                    └─► ConcreteHandler
```

> **Open/Closed Principle:** handlers know nothing about caching or logging. Behaviors are added as decorators without touching the handler.

### Layers and Responsibilities

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Entities, Value Objects with validation, `IRequest`, `IRequestHandler`, `ICachableRequest`, `IInvalidateCacheRequest` interfaces |
| **Application** | Commands/Queries (records), Handlers, `IMediator` interface |
| **Infrastructure** | `MyVanillaMediator` (via Reflection), Behaviors (Decorators), `InMemoryUserRepository`, automatic assembly scanning in DI |
| **Api** | REST Controllers, `Program.cs` |

---

## Implemented Use Cases

### Create a User — `POST /api/users`

```http
POST /api/users
Content-Type: application/json

{ "name": "Monkey D. Luffy", "age": 19 }
```

```json
// 201 Created
{ "id": "3fa85f64-...", "name": "Monkey D. Luffy", "age": 19 }
```

- Runs `CreateUserCommand` → `CreateUserHandler`
- Creates the `User` aggregate with a new `Guid`
- On completion, `InvalidateCacheBehavior` removes the `user-{id}` and `users-list` keys

---

### Get a User — `GET /api/users/{id}`

```http
GET /api/users/3fa85f64-...
```

```json
// 200 OK
{ "id": "3fa85f64-...", "name": "Monkey D. Luffy", "age": 19 }
```

- Runs `GetUserByIdQuery` → `GetUserByIdHandler`
- First call: **CACHE MISS** → queries the repository and stores the result for 5 minutes
- Subsequent calls: **CACHE HIT** → response served directly from `IMemoryCache`

---

### Update a User — `PUT /api/users/{id}`

- Runs `UpdateUserCommand` → `UpdateUserHandler`
- Loads the aggregate, calls `user.UpdateDetails(name, age)` following DDD
- `InvalidateCacheBehavior` automatically clears the user's cache

---

## Key Concepts

### MyVanillaMediator

Resolves the appropriate handler from `IServiceProvider` using Reflection and dispatches the request:

```csharp
var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
var handler     = _serviceProvider.GetRequiredService(handlerType);
```

### Automatic Registration (DependencyInjection.cs)

Scans the Application assembly, finds all concrete `IRequestHandler<,>` implementations, and automatically wraps them with decorators — no manual registration per use case:

```csharp
builder.Services.AddInfrastructure(typeof(CreateUserCommand).Assembly);
```

### Value Objects with Validation

```csharp
Age.Create(5);    // ❌ ArgumentException: Age must be between 13 and 99
Name.Create("");  // ❌ ArgumentException: Name cannot be null or empty
```

---

## How to Run

### Requirements
- .NET 8 SDK

### Start the API

```bash
dotnet run --project VainillaSystem.Api --launch-profile http
```

Swagger UI available at: `http://localhost:5239/swagger`

### Run the Tests

```bash
dotnet test VainillaSystem.sln --filter "FullyQualifiedName~UnitTests"
```

**Current result:** ✅ 39 tests — 0 failures

---

## Unit Tests

| Project | Covered Classes |
|---------|----------------|
| `Domain.UnitTests` | `EntityId`, `Age`, `Name`, `User` |
| `Application.UnitTests` | `CreateUserHandler`, `GetUserByIdHandler` |
| `Infrastructure.UnitTests` | `CachingBehavior`, `InvalidateCacheBehavior`, `LoggingBehavior`, `InMemoryUserRepository` |

Mocks are done with **NSubstitute**.

---

## Technologies

- [.NET 8](https://dotnet.microsoft.com/)
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core)
- [xUnit](https://xunit.net/)
- [NSubstitute](https://nsubstitute.github.io/)
- `Microsoft.Extensions.Caching.Memory`
- `Microsoft.Extensions.Logging.Abstractions`
