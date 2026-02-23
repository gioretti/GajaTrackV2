# Sleep Analysis Aggregation Design

**Date:** 2026-02-23
**Feature:** Sleep Trends Weekly/Monthly Aggregation

## Overview
The goal is to allow users to view their baby's sleep duration and patterns aggregated over weeks or months, rather than just daily data points. This helps in understanding longer-term trends in sleep duration and the shift between daytime naps and nighttime sleep.

## Constraints & Requirements
- Fast and responsive.
- Existing Scatter Plot charts (Nap vs. Night Wakings, Nap vs. Avg Night Session) must remain strictly daily, as averaging these correlations across a week/month obscures the insights.
- The top two charts (Area Trend and Line Trend) should update to show the aggregated data.
- X-Axis labels must clearly reflect the period (e.g., "Wk 42", "Oct").

## Selected Approach: Presentation ViewModel
We will introduce a view-model specific to the sleep trend charts, replacing the raw `DailyRhythmMapDay` DTO parameter in the top two chart components.

### 1. Data Contract
```csharp
public enum SleepAggregationMode
{
    Daily,
    Weekly,
    Monthly
}

public record SleepTrendDataPoint(
    string Label,            // Formatted date string (e.g., "14.10", "Wk 42", "Oct")
    double NightSleepMinutes,// Total or Average duration for the period
    double NapsMinutes       // Total or Average duration for the period
);
```

### 2. Component Updates
- **`SleepTrendChart.razor` & `SleepDurationLineChart.razor`**:
  - Change `[Parameter] public List<DailyRhythmMapDay> Days` to `[Parameter] public List<SleepTrendDataPoint> DataPoints`.
  - Update internal chart logic to read `.Label`, `.NightSleepMinutes`, and `.NapsMinutes` from the new record.

### 3. Page Logic (`SleepTrendPage.razor`)
- Add a `<select>` dropdown to choose the `SleepAggregationMode`.
- Retain the raw `_days` (`List<DailyRhythmMapDay>`) loaded from the API.
- Compute a derived `List<SleepTrendDataPoint> _aggregatedData` whenever `_days` or the selected aggregation mode changes.
- **Aggregation Logic**:
  - **Daily**: Direct mapping. Label: `dd.MM`.
  - **Weekly**: Group by ISO 8601 week. Label: `Wk XX`. Values: Average of daily values within that week.
  - **Monthly**: Group by Month/Year. Label: `MMM`. Values: Average of daily values within that month.
- Pass `_aggregatedData` to `SleepTrendChart` and `SleepDurationLineChart`.
- Pass raw `_days` to `SleepCorrelationChart` and `SleepSessionDurationCorrelationChart`.
