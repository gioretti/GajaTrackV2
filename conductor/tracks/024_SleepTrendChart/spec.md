# Specification: Sleep Trend Chart

## Problem Statement
While the Daily Rhythm Map shows *when* sleep happens, it is difficult for parents to visualize the *total duration* and the *balance* between daytime naps and consolidated night sleep across multiple days at a glance.

## Persona
**Parents** who want to track their baby's sleep progress and identify if the baby is getting enough total sleep or if daytime sleep is impacting nighttime rest.

## User Story
"As a parent, I want to see a stacked bar chart of my baby's sleep hours per day, split by category, so I can identify trends and patterns in sleep quality over time."

## Goals
- Add a new visualization component below the Daily Rhythm Map.
- Show clear daily totals for Naps vs. Night Sleep.
- Maintain visual alignment with the Rhythm Map's date axis.

## Acceptance Criteria

### 1. Visualization
- **Type:** Stacked Bar Chart.
- **Y-Axis:** Total hours (0 to 24).
- **X-Axis:** Aligned with the dates shown in the Daily Rhythm Map above it.
- **Stacks:**
    - **Night Sleep:** The base segment of the bar (e.g., Deep Blue).
    - **Naps:** The top segment of the bar (e.g., Sky Blue or Golden).

### 2. Integration
- The chart must be placed directly under the Daily Rhythm Map chart.
- It must respond to the same date range and "Prev/Next Page" navigation.
- It must use the data already provided by the `DailyRhythmMapDay` DTO (NapsMinutes and NightSleepMinutes).

### 3. Responsiveness
- The chart should scale horizontally to match the width of the Daily Rhythm Map above it.
- Hovering over a bar should ideally show the specific hours (e.g., "Night: 10h 15m, Naps: 2h 30m").

## Non-Goals
- Real-time data updates (reloading on navigation is sufficient).
- Advanced interactive zooming/panning (standard pagination is enough).
