# Implementation Plan: Sleep Categorization (TDD)

## Phase 0: Commit Boundaries
1. **Commit 1:** Domain Logic (Tests + Implementation).
2. **Commit 2:** Application Layer (Tests + Implementation).
3. **Commit 3:** Presentation Layer (UI Formatting).

## Phase 1: Domain Logic (TDD)
1. [ ] **Write failing tests** in `CalculateSleepTest.cs` for:
    - **Standard Nap:** 10:00-11:00 -> 60m Nap, 0m Night.
    - **Standard Night:** 22:00-02:00 -> 0m Nap, 240m Night.
    - **Boundary Nap:** 17:59-18:30 -> 31m Nap, 0m Night (Attributed by start time).
    - **Boundary Night:** 05:59-07:00 -> 0m Nap, 61m Night (Attributed by start time).
    - **Previous Day Exclusion:** Session starting at 05:50 (local) should not be counted in the current 06:00-06:00 day.
2. [ ] **Refactor `CalculateSleep.cs`**:
    - Return `(double NapsMinutes, double NightSleepMinutes)`.
    - Implement logic using `TimeZoneInfo` to determine attribution category.

## Phase 2: Application Layer (TDD)
1. [ ] **Update `DailyRhythmMapServiceTest.cs`** failing tests to expect categorized sleep.
2. [ ] **Update `DailyRhythmMapService.cs`**:
    - Update `DailyRhythmMapSummary` DTO properties.
    - Integrate the new `CalculateSleep` return tuple.

## Phase 3: Presentation Layer
1. [ ] **Update UI Components**:
    - Update `DailyRhythmMapSummaryCell.razor` to show "Naps" and "Night Sleep".
    - Ensure durations are formatted as `Xh Ym`.
