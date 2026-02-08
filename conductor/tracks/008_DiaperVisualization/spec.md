# Specification: Diaper Change Visualization

## Context
Parents need to monitor their baby's output (wet and solid diapers) to ensure proper health. Visualizing this on the 24-hour protocol helps identify patterns.

## User Story
**As a** parent
**I want** to see diaper changes represented as squares on the 24-hour protocol chart
**So that** I can track the frequency and type of waste (Wet/Solid) at a glance.

## Acceptance Criteria

### 1. Visual Representation
- [ ] **Point-in-time Event:** Diaper changes are rendered as point-in-time icons (like feedings).
- [ ] **Icon Shape:** Use a **Square**.
- [ ] **Color Coding:**
    -   **Wet** diapers: **Yellow** square (#FFD700 or similar).
    -   **Mixed** or **Soiled** diapers: **Brown** square (#8B4513 or similar).
- [ ] **Positioning:** Squares should be vertically centered in the row, similar to the triangles.

### 2. Legend Integration
- [ ] **Single Entry:** Add a single entry for "Diaper" in the legend.
- [ ] **Clarity:** The legend entry must show both the Yellow and Brown squares and clearly label what they mean (e.g., "Diaper (Yellow: Wet, Brown: Mixed/Solid)").
- [ ] **Visibility Toggle:** Add a checkbox to the "Diaper" legend entry to toggle visibility of all diaper squares in the chart.

### 3. Integration
- [ ] **Existing System:** Ensure the `ProtocolService` correctly maps `DiaperChange` entities to `ProtocolEvent`s (this seems already partially implemented but needs verification/styling support).
- [ ] **Filtering:** Update `ProtocolPage.razor` and `ProtocolChart.razor` to respect the visibility toggle for the `Diaper` type.
