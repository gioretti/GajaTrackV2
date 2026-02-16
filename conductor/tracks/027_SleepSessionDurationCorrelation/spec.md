# Track Specification: Sleep Session Duration Correlation

## Overview
Implement a second scatter plot in the Sleep Trends page to visualize the correlation between daytime nap duration and the average duration of night sleep sessions.

## User Story
As a parent, I want to know if long naps during the day lead to shorter individual sleep stretches at night, so I can better manage the daily schedule.

## Acceptance Criteria
1.  **Component:** A new Razor component `SleepSessionDurationCorrelationChart.razor` is created.
2.  **Visualization:**
    -   X-Axis: Daytime nap duration (0 to 12h).
    -   Y-Axis: Average duration of night sleep sessions (0 to Max found, minimum 6h).
    -   Data Points: Each day with at least one night sleep session is represented by an orange circle.
3.  **Logic:**
    -   Night window is defined as starting between 18:00 and 06:00 (StartMinute >= 720).
    -   Calculation is performed in the frontend (Approach 2).
    -   Days with zero night sleep sessions are omitted.
4.  **Aesthetics:**
    -   Orange circles (`#ffb300`) with 50% opacity.
    -   Consistent with existing chart grid styling.
5.  **Integration:** Positioned below the Waking Correlation chart on the Sleep Trends page.
