# Specification: Protocol Visibility Toggles

## Context
The 24-hour protocol visualization can become visually dense with all activity types (Sleep, Crying, Feeding) shown at once. Providing a way to filter these views helps parents focus on specific patterns.

## User Story
**As a** parent
**I want** to toggle the visibility of individual activity types via the chart legend
**So that** I can declutter the chart and focus on the data that matters most to me at any given time.

## Acceptance Criteria

### 1. Legend Toggles
- [ ] **Interactive Legend:** Each item in the chart legend (Sleep, Crying, Nursing, Formula, Bottle) must have a checkbox next to it.
- [ ] **Default State:** All checkboxes are checked by default (everything visible).
- [ ] **Visibility Control:** Toggling a checkbox immediately hides or shows the corresponding visual elements within the 24-hour protocol grid.

### 2. Supported Types
The following types must be independently toggleable:
- Sleep (Horizontal lines)
- Crying (Wavy lines)
- Nursing (Hollow triangles)
- Formula (Solid blue triangles)
- Bottle (Solid grey triangles)

### 3. Impact & Scope
- [ ] **No Summary Impact:** Toggling visibility must **NOT** affect the "Daily Summary" column (the totals remain calculated and displayed regardless of visual state).
- [ ] **Local State:** Toggles reset to "All On" when the page is reloaded (no persistence required).
- [ ] **UI Sync:** The legend icons/text should remain visible even when the data type is toggled off in the chart.

### 4. Implementation Constraints
- The implementation must handle the filtering logic within the Blazor components (`ProtocolChart.razor` and its host).
