# Specification: Sleep Categorization (Naps vs. Night Sleep)

## Problem Statement
The application currently only provides a "Total Sleep" duration for a 24-hour logical day (06:00 to 06:00). Parents need more granular metrics to understand the quality of rest, specifically distinguishing between daytime naps and consolidated night sleep.

## Goals
- Extend `CalculateSleep` to categorize sleep sessions.
- Attribution must be based on the session's **Start Time**.
- Display separate "Naps" and "Night Sleep" durations in the UI summary.

## Acceptance Criteria

### 1. Attribution Logic (Local Time)
- **Daily Naps:** Any sleep session that starts between **06:00:00 and 17:59:59**.
- **Night Sleep:** Any sleep session that starts between **18:00:00 and 05:59:59**.
- **No Clipping:** The full duration of the session is attributed to its category based on the start time, even if it crosses a boundary.

### 2. Domain Service Extension
- `CalculateSleep` must return a result containing:
    - Total Sleep duration.
    - Night Sleep duration.
    - Daily Naps duration.

### 3. UI Display
- The "Summary" section for each day in the Daily Rhythm Map must show:
    - **Naps:** [Duration]
    - **Night Sleep:** [Duration]
    - **Total:** [Duration] (Sum of Naps + Night)

### 4. TDD
- Unit tests must verify attribution for:
    - A nap starting at 17:59 (Full duration categorized as Nap).
    - Night sleep starting at 05:59 (Full duration categorized as Night).
    - Standard sessions well within boundaries.
