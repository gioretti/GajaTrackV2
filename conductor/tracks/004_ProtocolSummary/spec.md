# Specification: Daily Summary Column (Protocol Aligned)

## Context
Parents need to see summary metrics (initially Total Sleep) alongside the visual 24-hour protocol to quickly assess the day's quality. This summary must strictly respect the 06:00-06:00 "Protocol Day" definition.

## User Story
**As a** Parent tracking my baby's behavior
**I want** to see a daily summary column on the right side of the 24-hour protocol
**So that** I can see the total sleep duration for that specific "Protocol Day" (06:00 AM - 06:00 AM next day).

## Acceptance Criteria

### 1. Layout & Alignment
- [ ] **Column Addition:** A "Summary" column is added to the right of the 24-hour protocol grid.
- [ ] **Row Alignment:** Each summary cell is vertically aligned with its corresponding **Protocol Day** row.

### 2. Data - Protocol Sleep Total
- [ ] **Strict Time Window:** Calculate total sleep duration *strictly* within the **06:00 to 06:00** window for that row.
- [ ] **Boundary Splitting:**
    -   Sleep crossing the start (06:00) counts only the portion *after* 06:00 for that day.
    -   Sleep crossing the end (06:00 next day) counts only the portion *before* 06:00 for that day.
- [ ] **Formatting:** Display as "Sleep: Xh Ym" (e.g., "Sleep: 14h 30m").

### 3. UI/UX & Extensibility
- [ ] **Space Optimization:** The design must be compact.
- [ ] **Extensible Container:** The summary cell must be implemented as a container (e.g., Flexbox column or Stack) capable of holding multiple items, even though only "Sleep" is populated now.

## Out of Scope
- Adding "Night Wakings" or other metrics (these are for future stories).
