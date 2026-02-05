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

## Atomic Tasks

### 1. Application Layer: DTOs & Interface
- [ ] Create `ProtocolDTOs.cs`: `ProtocolDay`, `ProtocolEvent` (Type, StartTime, Duration, Details).
- [ ] Create `IProtocolService.cs`: Interface for fetching protocol data.
- [ ] Define Event Types: Sleep, Crying, Feed (Nursing, Bottle, Formula).

### 2. Infrastructure Layer: Protocol Service Implementation
- [ ] Implement `ProtocolService.cs`.
- [ ] **Logic**: Fetch data from `DbContext`.
- [ ] **Logic**: Map entities to `ProtocolEvent`s.
- [ ] **Logic**: "Day Splitting". Ensure events spanning across 06:00 are split or assigned to the correct "Protocol Day" (06:00 - 06:00).
- [ ] Register Service in DI.

### 3. Tests: Service Logic
- [ ] Unit Test: Verify 06:00 boundary handling (e.g., sleep from 05:00 to 07:00 should split).
- [ ] Unit Test: Verify correct day assignment (Events before 06:00 belong to previous day).

### 4. UI Layer: Components
- [ ] Create `ProtocolChart.razor`: The SVG-based visualizer.
- [ ] Implement Grid (Time Axis 06:00 - 06:00).
- [ ] Implement Row Rendering (Loop through days).
- [ ] Implement Shapes (Rect for Sleep, Path for Crying, Triangle for Feed).
- [ ] Add `ProtocolPage.razor` to host the chart.
