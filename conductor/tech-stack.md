# Technology Stack: Baby Gaja Tracking Application

## Core Platform
- **Runtime:** .NET 9.0 (Cross-platform)
- **Primary Language:** C# 13

## Architecture & Frameworks
- **Pattern:** Clean Architecture / DDD (Domain-Driven Design)
- **Frontend:** Blazor (Currently utilizing a hybrid WebAssembly/Server model)
- **Backend:** .NET 9.0 (Integrated; potential transition to decoupled Web API under consideration)
- **Object Mapping/Serialization:** System.Text.Json

## Data Persistence
- **Database:** SQLite (Chosen for local-first speed and portability)
- **ORM:** Entity Framework Core 9.0
- **Identity:** UUIDv7 (For sortable, unique identifiers)

## Quality & Infrastructure
- **Dependency Injection:** Microsoft.Extensions.DependencyInjection
- **Testing:** xUnit / FluentAssertions (Inferred)
- **Logging:** Microsoft.Extensions.Logging

## Infrastructure Decisions (ADRs)
- **ADR-001:** SQLite for local persistence.
- **ADR-002:** UUIDv7 for entity identity.
- **ADR-003:** UTC for all date/time persistence to ensure consistency across time zones.
