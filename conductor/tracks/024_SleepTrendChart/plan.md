# Implementation Plan: Sleep Trend Chart

## Phase 0: Commit Boundaries
1. **Commit 1:** Create `SleepTrendChart.razor` component and basic layout.
2. **Commit 2:** Integrate chart into `DailyRhythmMapPage.razor` and polish visuals.

## Phase 1: Chart Component (Commit 1)
1. [ ] Create `SleepTrendChart.razor` in `WebApp/Components/Pages/DailyRhythmMap/`.
2. [ ] **Parameters:** Accept `List<DailyRhythmMapDay> Days`.
3. [ ] **SVG Layout:**
    - **Width:** Match `DailyRhythmMapChart` (approx 1600px).
    - **Height:** Fixed height (e.g., 300px).
    - **X-Axis:** 14 slots for the 14 days.
    - **Y-Axis:** Scale 0 to 24 hours.
4. [ ] **Rendering Logic:**
    - For each day, draw a stacked bar:
        - `NightHeight = (NightSleepMinutes / 1440) * ChartHeight`
        - `NapHeight = (NapsMinutes / 1440) * ChartHeight`
    - Use contrasting colors:
        - **Night:** Deep Indigo (`#1a237e`)
        - **Naps:** Golden Amber (`#ffb300`)
5. [ ] **Labels:** Add Y-axis labels (0h, 6h, 12h, 18h, 24h) and X-axis date labels.

## Phase 2: Integration & Polish (Commit 2)
1. [ ] Update `DailyRhythmMapPage.razor`:
    - Place `<SleepTrendChart Days="_days" />` below the `DailyRhythmMapChart`.
2. [ ] **Alignment:** Ensure the chart containers share the same margins and shadows for visual consistency.
3. [ ] **Hover/Tooltips:** Add basic SVG `title` tags to bars for quick duration checks.
4. [ ] **Styling:** Ensure colors are distinct from the event lines in the Rhythm Map to avoid confusion.
