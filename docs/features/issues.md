# Feature Implementation Issues

This document tracks an discrepancies found between intended features/specs and the actual source code implementation during the documentation phase.

## Data Export
- **Missing Entity:** The `ExportService.cs` does not include `CryingSession` records in the generated JSON export. However, the Baby+ Data Import feature explicitly supports importing Crying Sessions. This creates an asymmetry where imported data cannot be fully exported.

## Sleep Analysis
- **Duplicated/Inconsistent Logic:** The correlation charts (`SleepCorrelationChart.razor` and `SleepSessionDurationCorrelationChart.razor`) compute "Night Wakings" and "Night Sessions" using client-side filtering logic (`StartMinute >= 720` and `Duration >= 20m`). This bypasses the more robust `NightWakings` count already provided by the server in `day.Summary.NightWakings`, which relies on `BabyDay.NightTimeStart`.
