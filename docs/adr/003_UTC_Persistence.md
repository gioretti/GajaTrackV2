# ADR 003: UTC Date Persistence
status: Accepted
date: 2026-02-06 (Updated)

## Context
Baby tracking data is time-sensitive. Users may travel across timezones. Persisting dates in Local Time causes ambiguity (e.g., during Daylight Savings transitions). While the initial decision was to use `DateTime` with UTC kind, this relies on developer discipline and is prone to "Local" time leakage.

## Decision
1.  All timestamps in the **Domain** must use a specialized **`UtcDateTime` Value Object** instead of the primitive `DateTime`.
2.  The `UtcDateTime` type will strictly enforce `DateTimeKind.Utc` during construction.
3.  **Persistence** will use EF Core `ValueConverters` to map this type to the database.

## Rationale
-   **Type Safety**: Prevents passing `DateTime.Now` (Local) to Domain entities at compile-time/construction.
-   **Unambiguous**: UTC provides a single point of truth regardless of server location or user timezone.
-   **Encapsulation**: Centralizes all time-related logic (validation, rounding) within the Value Object.

## Consequences
-   **UI Responsibility**: The Presentation layer (Blazor) remains responsible for converting UTC to the user's Local Time for display.
-   **Mapping Cost**: Requires a one-time configuration in `GajaDbContext`.
-   **Refactoring**: Existing entities and services must be updated to use the new type.
