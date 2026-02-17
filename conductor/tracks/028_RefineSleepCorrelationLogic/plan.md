# Implementation Plan: Refine Sleep Correlation Logic

## Phase 1: Logic Implementation
1.  [ ] Update `SleepCorrelationChart.razor` to filter night sessions < 20m and recalculate Y-axis.
2.  [ ] Update `SleepSessionDurationCorrelationChart.razor` to filter night sessions < 20m and recalculate Y-axis.
3.  [ ] Update tooltips in both components to indicate "Filtered" values.

## Phase 2: Verification
1.  [ ] Build check.
2.  [ ] Manual UI verification via tooltips to confirm 20m threshold is active.
3.  [ ] Request Review (STOP Phase).
