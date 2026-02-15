# Implementation Plan: 018_BabyDayDomainModel

The goal is to introduce a `BabyDay` domain object and separate calculation services to move summary logic into the Domain layer and centralize data fetching via a query service.

## Proposed Changes

### 1. Domain Layer (`src/GajaTrack.Domain`)

#### [NEW] `BabyDay.cs`
- Immutable record/class containing:
    - `DateOnly Date`
    - `TimeRange Window`
    - Collections of: `SleepSession`, `CryingSession`, `NursingFeed`, `BottleFeed`, `DiaperChange`.

#### [NEW] `CalculateSleep.cs`
- Method: `double For(BabyDay day)`
- Logic: Sum of durations of all sleep sessions in `day`, clipped to `day.Window`.

#### [NEW] `CountWakings.cs`
- Method: `int For(BabyDay day, TimeOnly from, TimeOnly to)`
- Logic: Count sessions in `day` that end between `from` and `to` (local time), excluding the very last session of the day.

### 2. Application Layer (`src/GajaTrack.Application`)

#### [NEW] `Queries/GetBabyDayQuery.cs`
- Fetches all data for the requested range.
- Constructs `BabyDay` objects, ensuring overlapping events are added to all intersecting days.

#### `DailyRhythmMapService.cs`
- Refactor to call `GetBabyDayQuery`.
- Use `CalculateSleep` and `CountWakings` to populate DTOs.

### 3. Test Suite (`tests/GajaTrack.Tests.Unit`)

#### [NEW] `Domain/CalculateSleepTest.cs`
- Test clipping logic (start/end boundaries).

#### [NEW] `Domain/CountWakingsTest.cs`
- Test time-range logic and last-session exclusion.

#### [NEW] `Application/GetBabyDayQueryTest.cs`
- Test data orchestration and overlapping event assignment.

---

## Verification Plan

### Automated Tests
- `dotnet test`

### Manual Verification (MANDATORY)
- Verify Daily Rhythm Map consistency in the browser.
