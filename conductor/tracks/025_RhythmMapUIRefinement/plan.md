# Implementation Plan: Daily Rhythm Map UI Refinement

## Phase 0: Commit Boundaries
1. **Commit 1:** Refactor `DailyRhythmMapSummaryCell.razor` to two-column layout.
2. **Commit 2:** Reduce `RowHeight` in `DailyRhythmMapChart.razor` and polish.

## Phase 1: Summary Cell (Commit 1)
1. [ ] Update `DailyRhythmMapSummaryCell.razor`:
    - Refactor SVG layout to use two columns.
    - Column 1 (x=0): Total, Wakings.
    - Column 2 (x=80 or similar offset): Naps, Night.
    - Reduce vertical line spacing.

## Phase 2: Chart Dimensions (Commit 2)
1. [ ] Update `DailyRhythmMapChart.razor`:
    - Change `RowHeight` from 75 to **45**.
    - Adjust `SummaryWidth` if necessary to accommodate two columns.
    - Verify vertical centering of date labels and event markers.

## Phase 3: Verification
1. [ ] Manual UI verification on port 5000.
