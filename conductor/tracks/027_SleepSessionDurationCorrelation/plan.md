# Implementation Plan: Sleep Session Duration Correlation

## Phase 1: Component Implementation
1.  [ ] Create `src/GajaTrack.Presentation/GajaTrack.WebApp/Components/Pages/DailyRhythmMap/SleepSessionDurationCorrelationChart.razor`.
2.  [ ] Implement logic to calculate average night session duration per day.
3.  [ ] Render SVG scatter plot with orange circles and tooltips.

## Phase 2: Page Integration
1.  [ ] Add `<SleepSessionDurationCorrelationChart Days="_days" />` to `SleepTrendPage.razor`.

## Phase 3: Verification
1.  [ ] Build check.
2.  [ ] Manual UI verification of the calculated average values.
3.  [ ] Request Review (STOP Phase).
