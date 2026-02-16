# Design Doc: Refined Sleep Correlation Logic

## 1. Overview
This refinement introduces a 20-minute minimum threshold for night-time sleep sessions used in the correlation diagrams. This filter helps eliminate "noise" from very short wakings or false-start sleep sessions, providing a clearer picture of significant sleep patterns.

## 2. Shared Filter Logic
- **Threshold:** 20 minutes.
- **Scope:** Night sessions only (`StartMinute >= 720`, which corresponds to 18:00 local time).
- **Implementation:** Performed in the frontend within the Razor component's local methods.

## 3. Component Updates

### Waking Correlation Chart
- **X-Axis:** Remains `day.Summary.NapsMinutes` (Unfiltered).
- **Y-Axis Calculation:**
    1. Filter night sleep events where `DurationMinutes >= 20`.
    2. Result: `Math.Max(0, filteredSessions.Count - 1)`.
- **Tooltip:** Displays "Filtered Wakings" to distinguish from the raw data.

### Session Duration Correlation Chart
- **X-Axis:** Remains `day.Summary.NapsMinutes` (Unfiltered).
- **Y-Axis Calculation:**
    1. Filter night sleep events where `DurationMinutes >= 20`.
    2. Average the duration of these specific sessions.
    3. Skip the day if no sessions meet the threshold.
- **Tooltip:** Displays "Avg Filtered Session" length.

## 4. Verification Plan
- **Build Check:** Ensure no syntax errors.
- **Manual Verification:** Compare tooltips between the standard "Rhythm Map" summary and the correlation charts to confirm the filter is active.
