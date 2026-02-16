# Implementation Plan: Sleep Correlation Chart

## Phase 1: Component Implementation
1.  [ ] Create `src/GajaTrack.Presentation/GajaTrack.WebApp/Components/Pages/DailyRhythmMap/SleepCorrelationChart.razor`.
2.  [ ] Implement SVG coordinate mapping for X (Naps) and Y (Wakings).
3.  [ ] Render grid lines and axis labels.
4.  [ ] Render scatter points (circles) with tooltips.

## Phase 2: Page Integration
1.  [ ] Add `<SleepCorrelationChart Days="_days" />` to `SleepTrendPage.razor`.
2.  [ ] Adjust layout spacing for the third chart.

## Phase 3: Verification
1.  [ ] Final build and test run.
2.  [ ] Manual UI verification via Chrome DevTools.
3.  [ ] Request Review (STOP Phase).
