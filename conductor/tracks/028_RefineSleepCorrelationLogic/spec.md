# Track Specification: Refine Sleep Correlation Logic

## Overview
Update the Sleep Correlation diagrams to exclude "noise" by filtering out night-time sleep sessions that last less than 20 minutes.

## User Story
As a parent, I want to exclude short night-time wake-ups or false starts from my correlation data, so I can see significant trends more clearly.

## Acceptance Criteria
1.  **Filter Logic:** A 20-minute minimum duration threshold is applied to all night-time sleep sessions (`StartMinute >= 720`) used in correlation calculations.
2.  **Waking Correlation Chart:**
    -   Y-axis (Wakings) only counts sessions >= 20 mins.
    -   Excludes the last qualifying wake-up of the night.
    -   Tooltip shows "Filtered Wakings".
3.  **Session Duration Correlation Chart:**
    -   Y-axis (Avg Duration) only averages sessions >= 20 mins.
    -   Omit days with zero qualifying night sessions.
    -   Tooltip shows "Avg Filtered Session".
4.  **Scope:** These filters apply ONLY to the two correlation scatter plots, not to the daily rhythm map or other charts.
