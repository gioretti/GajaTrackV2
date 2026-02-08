# Plan: Refactor Protocol Summary to Standalone Component

## Phase 1: Structural Refactoring
- [x] **Task 1.1: Create ProtocolSummaryCell.razor**
  - Extract the SVG rendering logic for the summary into this new component.
  - Define `ProtocolSummary` as a `[Parameter]`.
- [x] **Task 1.2: Integrate into ProtocolChart.razor**
  - Replace the inline logic in `ProtocolChart.razor` with a call to `<ProtocolSummaryCell Summary="day.Summary" X="..." Y="..." />`.
- [x] **Task 1.3: Verification**
  - Ensure the build succeeds.
  - Manual UI Verification using Chrome DevTools to confirm zero visual regressions.

## Phase 2: Final Review
- [ ] **Task 2.1: Architectural Review**
  - Submit Implementation Summary to Architect.
- [ ] **Task 2.2: Complete Track**
  - Update `conductor/tracks.md`.
