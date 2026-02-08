# Specification: Night Wakings Metric

## Context
Parents need to track how often their baby wakes up during the night to assess sleep quality and developmental progress. This metric should be displayed in the daily summary column.

## User Story
**As a** Parent
**I want** to see the count of night wakings in the daily summary
**So that** I can track sleep interruptions during the "Night" window (18:00 - 06:00).

## Acceptance Criteria

### 1. Definition & Calculation
- [ ] **Night Window:** Defined as **18:00** of the Protocol Day to **06:00** of the next morning (the end of the Protocol Day).
- [ ] **Waking Definition:** A "night waking" is an interruption during the Night Window. 
- [ ] **Calculation Logic:** Count every **Sleep Session end** that occurs within the Night Window (18:00 <= End Time < 06:00), **EXCEPT for the very last sleep session of the protocol day.**
- [ ] **Boundary Handling:** If a day has only one sleep session that ends at 05:45, it is considered the start of the day and not an interruption, so the count is 0.
- [ ] **Constraint:** Only sleep sessions belonging to the current Protocol Day's grid are considered.

### 2. UI/UX Display
- [ ] **Location:** The metric is displayed as a new row in the `ProtocolSummaryCell`.
- [ ] **Labeling:** The label must clearly state the timeframe: **"Night Wakings (18-06): [Count]"**.
- [ ] **Formatting:** Use the same font size and styling as the "Sleep" total for consistency.
- [ ] **Empty State:** if the count is 0, it should display "Night Wakings (18-06): 0".

### 3. Data Integrity
- [ ] The calculation must be performed in the `ProtocolService` to ensure consistency between the visual bars and the summary count.
