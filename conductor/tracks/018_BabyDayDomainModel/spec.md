# Track Specification: 018_BabyDayDomainModel

## Problem Statement
The logic for defining a "Baby Day" (06:00 to 06:00 local time) and calculating summaries for the Daily Rhythm Map is currently scattered. This leads to "logic leaks" and makes it difficult to reason about the daily rhythm as a unified domain concept.

## Goal
Introduce a `BabyDay` domain object to encapsulate a 24-hour period. Implement dedicated **Domain Services** for analytical calculations and a `GetBabyDayQuery` application service to fetch these objects.

## Acceptance Criteria
1.  **Domain Model:**
    - Create a `BabyDay` object in `GajaTrack.Domain` to hold the date, `TimeRange` window, and entity collections.
    - A single event intersecting boundaries must be included in all relevant `BabyDay` instances.
2.  **Domain Services:**
    - `CalculateSleep.For(babyDay)`: Returns total sleep minutes clipped to the day's window.
    - `CountWakings.For(babyDay, from, to)`: Returns the count of interruptions within the specified time range.
3.  **Query Service:**
    - Implement `GetBabyDayQuery` in the Application layer to orchestrate data fetching and `BabyDay` construction.
4.  **Unit Testing:**
    - TDD implementation for `CalculateSleep`.
    - TDD implementation for `CountWakings`.
    - TDD implementation for `GetBabyDayQuery`.
5.  **Refactoring:**
    - `DailyRhythmMapService` must use `GetBabyDayQuery` and the new Domain Services.
6.  **Manual Verification:**
    - Verify the Daily Rhythm Map renders correctly in the browser.

## Constraints
- Adhere to Clean Architecture and DDD.
- Respect `UtcDateTime` and `TimeRange` value objects.
- No persistence logic in Domain.
