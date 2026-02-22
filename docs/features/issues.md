# Feature Implementation Issues

This document tracks an discrepancies found between intended features/specs and the actual source code implementation during the documentation phase.


## Sleep Analysis
- **Duplicated/Inconsistent Logic:** The correlation charts (`SleepCorrelationChart.razor` and `SleepSessionDurationCorrelationChart.razor`) compute "Night Wakings" and "Night Sessions" using client-side filtering logic (`StartMinute >= 720` and `Duration >= 20m`). This bypasses the more robust `NightWakings` count already provided by the server in `day.Summary.NightWakings`, which relies on `BabyDay.NightTimeStart`.
