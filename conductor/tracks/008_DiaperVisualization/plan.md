# Plan: Diaper Change Visualization

Add color-coded squares to represent diaper changes on the 24-hour protocol chart.

## Architectural Notes
- **Application Layer:** The mapping logic in `ProtocolService.cs` already exists but needs verification to ensure the `DiaperType` string is passed correctly in the `Description` field of `ProtocolEvent`.
- **UI Layer:** 
  - `ProtocolPage.razor`: Update legend and visibility state.
  - `ProtocolChart.razor`: Implement square rendering with color logic based on diaper type.

## Phase 1: UI State & Legend
- [ ] **Task 1.1: Update Visibility State**
  - Add `ProtocolEventType.Diaper` to the `_visibleTypes` set in `ProtocolPage.razor`.
- [ ] **Task 1.2: Update Legend UI**
  - Add a "Diaper" legend entry with a checkbox.
  - Show two squares: one Yellow (Wet) and one Brown (Mixed/Solid).
  - Add a clear text label explaining the colors.

## Phase 2: Chart Rendering
- [ ] **Task 2.1: Implement Square Rendering in ProtocolChart**
  - Add `case ProtocolEventType.Diaper` to the rendering loop in `ProtocolChart.razor`.
  - Render a `<rect>` (square) centered vertically.
  - Logic for `fill` color:
    - If `Description` contains "Wet" -> Yellow (#FFD700).
    - If `Description` contains "Mixed" or "Soiled" -> Brown (#8B4513).
- [ ] **Task 2.2: Manual UI Verification**
  - Start the application (using detached process protocol).
  - Verify diaper squares appear on days with diaper data.
  - Verify Yellow/Brown colors match the data types.
  - Verify the "Diaper" checkbox toggles visibility correctly.

## Phase 3: Final Review
- [ ] **Task 3.1: Architectural Review**
- [ ] **Task 3.2: Complete Track**
