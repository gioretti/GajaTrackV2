# Implementation Plan: Sleep Trend Chart (Dedicated Page)

## Phase 0: Commit Boundaries
1. **Commit 1:** Create `SleepTrendChart.razor` component.
2. **Commit 2:** Create `SleepTrendPage.razor` and add navigation link.
3. **Commit 3:** Clean up `DailyRhythmMapPage.razor`.

## Phase 1: Chart Component (Done)
1. [x] Create `SleepTrendChart.razor` in `WebApp/Components/Pages/DailyRhythmMap/`.

## Phase 2: Dedicated Page (Commit 2)
1. [ ] Create `SleepTrendPage.razor` in `WebApp/Components/Pages/DailyRhythmMap/`.
    - Implement same data fetching logic as Rhythm Map (14 days, pagination).
    - Render `<SleepTrendChart Days="_days" />`.
2. [ ] Update `NavMenu.razor`:
    - Add link to "Sleep Trends".

## Phase 3: Cleanup (Commit 3)
1. [ ] Remove `<SleepTrendChart>` and associated container from `DailyRhythmMapPage.razor`.
2. [ ] Revert `DailyRhythmMapPage` to focus only on the 24h protocol.
