# ADR 003: UTC Date Persistence
status: Accepted
date: 2026-01-30

## Context
Baby tracking data is time-sensitive. Users may travel across timezones. Persisting dates in Local Time causes ambiguity (e.g., during Daylight Savings transitions) and makes data exchange difficult.

## Decision
All `DateTime` values in the **Domain** and **Persistence** layers must be stored and manipulated as **UTC**.

## Rationale
-   **Unambiguous**: UTC provides a single point of truth regardless of server location or user timezone.
-   **Standardization**: Unix timestamps (from our import source) are natively UTC.
-   **Simplicity**: Calculations (Duration = End - Start) work correctly without timezone offsets.

## Consequences
-   **UI Responsibility**: The Presentation layer (Blazor) is responsible for converting UTC to the user's Local Time for display.
-   **Input Responsibility**: When a user inputs "10:00 AM", the UI must convert it to UTC before sending to the Application layer.
