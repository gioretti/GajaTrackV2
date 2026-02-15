# Implementation Plan: Sleep Categorization Refinement

## Phase 0: Commit Boundaries
1. **Commit 1:** Guideline updates and Domain constants.
2. **Commit 2:** Domain Service refactor (Result object and naming).
3. **Commit 3:** DTO and UI refactor (Removing redundancy).
4. **Commit 4:** Test cleanup and variable renaming.

## Phase 1: Guidelines & Constants (Commit 1)
1. [ ] Update `conductor/code_styleguides/csharp.md` with rules against Tuples and for Constants.
2. [ ] Update `BabyDay.cs`:
    - Define `public const int DayTimeStart = 6;`
    - Define `public const int NightTimeStart = 18;`
    - Use these in `CalculateTimeBounds`.

## Phase 2: Domain Refactor (Commit 2)
1. [ ] Update `CalculateSleep.cs`:
    - Define `public record Result(double NapsMinutes, double NightSleepMinutes);`
    - Rename `IsInNapWindow` -> `IsDayTime`.
    - Use `BabyDay.DayTimeStart` and `BabyDay.NightTimeStart` constants.

## Phase 3: DTO & UI (Commit 3)
1. [ ] Update `DailyRhythmMapDtos.cs`: Remove `TotalSleepMinutes`.
2. [ ] Update `DailyRhythmMapService.cs` to match.
3. [ ] Update `DailyRhythmMapSummaryCell.razor` to calculate Total on the fly.

## Phase 4: Test Cleanup (Commit 4)
1. [ ] Rename `ev` -> `sleepEvent` in all tests.
2. [ ] Rename ambiguous test methods in `DailyRhythmMapServiceTest.cs`.
3. [ ] Update `CalculateSleepTest.cs` to use the `Result` object.
