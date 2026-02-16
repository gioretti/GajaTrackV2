# Track Specification: Sleep Correlation Chart

## Overview
Implement a scatter plot diagram in the Sleep Trends page to visualize the correlation between daytime naps and night wakings.

## User Story
As a parent, I want to see if my baby's daytime sleep duration affects how many times they wake up at night, so I can optimize their sleep schedule.

## Acceptance Criteria
1.  **Component:** A new Razor component `SleepCorrelationChart.razor` is created.
2.  **Visualization:** 
    -   X-Axis: Hours of daytime nap sleep (0 to 12h).
    -   Y-Axis: Number of night wakings (0 to Max in set).
    -   Data Points: Each day in the last 30 days is represented by a circle.
3.  **Aesthetics:**
    -   Blue circles with 50% opacity.
    -   Standard grid lines for axes.
    -   Consistent with existing `SleepTrendChart` styling.
4.  **Information:** Tooltips (SVG title) show date, nap duration, and waking count.
5.  **Integration:** The chart is displayed at the bottom of the Sleep Trends page.
