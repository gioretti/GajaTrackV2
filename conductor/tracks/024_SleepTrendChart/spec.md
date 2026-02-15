# Specification: Sleep Trend Chart

## Problem Statement
While the Daily Rhythm Map shows *when* sleep happens, it is difficult for parents to visualize the *total duration* and the *balance* between daytime naps and consolidated night sleep across multiple days at a glance. Placing both charts on one page makes it cluttered.

## Persona
**Parents** who want to track their baby's sleep progress and identify if the baby is getting enough total sleep or if daytime sleep is impacting nighttime rest.

## User Story
"As a parent, I want to see a dedicated Sleep Trends page with a stacked bar chart of my baby's sleep hours per day, so I can identify trends and patterns in sleep quality over time without the clutter of the daily map."

## Goals
- Add a new **Sleep Trends** page to the application.
- Show clear daily totals for Naps vs. Night Sleep using a stacked bar chart.
- Add navigation to this new page from the main menu.

## Acceptance Criteria

### 1. New Page
- **Route:** `/sleep-trends`.
- **Title:** "Sleep Duration Trends".
- **Navigation:** Must be accessible from the side/top navigation menu.

### 2. Visualization
- **Type:** Stacked Bar Chart.
- **Y-Axis:** Total hours (0 to 24).
- **Stacks:**
    - **Night Sleep:** The base segment of the bar (Deep Indigo).
    - **Naps:** The top segment of the bar (Golden Amber).

### 3. Integration
- The page must have its own date navigation (Prev/Next Page) showing 14 days at a time.
- It must use the data already provided by the `DailyRhythmMapDay` DTO.

### 4. Responsiveness
- Hovering over a bar should show the specific hours.
- The chart should be the primary focus of the page.
