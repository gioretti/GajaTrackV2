# User Story 003: 24-Hour Sleep & Activity Protocol Visualization

## Context
Based on the clinical "24-Stunden-Protokoll" used by the University Children's Hospital Zurich.
- **Reference PDF**: [24-stunden-protokoll.pdf](./attachments/24-stunden-protokoll.pdf)
- **AI Description**: [24-stunden-protokoll-ai-description.md](./attachments/24-stunden-protokoll-ai-description.md)

## Story
**As a** Sleep-deprived Parent
**I want** to see a standardized 24-hour horizontal grid of my baby's activities (Sleep, Feed, Cry)
**So that** I can instantly spot sleep patterns and share the standard "medical view" with a pediatrician without drawing it manually.

## Acceptance Criteria

### 1. Grid Structure (The Canvas)
- [ ] **Time Axis**: Horizontal axis runs strictly from **06:00** (Current Day) to **06:00** (Next Day).
- [ ] **Day Rows**: Each calendar day is a separate row.
- [ ] **Readability**:
    -   **Row Banding**: Alternating background colors for rows (Zebra striping) to ease the eye.
    -   **Column Banding**: Ideally, hours also alternate background colors (checkerboard or subtle vertical striping) to assist vertical scanning.
- [ ] **Legend**: A clear legend explaining all symbols (Sleep, Cry, Feed Types) is visible.

### 2. Feeding Visualization (Triangles)
- [ ] **Nursing**: Displayed as an **Empty (Hollow) Triangle** at the *Start Time*.
- [ ] **Formula**: Displayed as a **Solid Blue Triangle**.
- [ ] **Bottle (Breast Milk)**: Displayed as a **Solid Grey Triangle**.
- [ ] **Constraint**: Ignore "End Time" for feedings; render as point-in-time events.

### 3. Behavior Visualization
- [ ] **Sleep**: Displayed as a continuous **Horizontal Line** spanning from Start Time to End Time.
- [ ] **Crying**: Displayed as a **Wavy Line**.
    -   **Visual Guarantee**: Even short crying episodes (e.g., 2 mins) must be rendered with "full amplitude" (minimum visual width/height) so they are clearly distinguishable.
- [ ] **Exclusion**: Do **NOT** render "Bedtime" arrows (deviation from PDF).

### 4. Data Handling
- [ ] **Cross-Day Events**: Events spanning across the 06:00 boundary must visually wrap or be handled such that the visualization remains accurate to the 06:00-06:00 cycle.
- [ ] **Mobile View**: Horizontal scrolling is acceptable/preferred over compressing the 24h axis into unreadable widths.
