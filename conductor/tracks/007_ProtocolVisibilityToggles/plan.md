# Plan: Protocol Visibility Toggles

Add checkboxes to the 24-hour protocol legend to allow showing/hiding specific activity types in the chart.

## Architectural Notes
- **State Management:** The visibility state (which types are shown) will be managed in `ProtocolPage.razor` using a `HashSet<ProtocolEventType>`.
- **Component Communication:** The visibility state will be passed as a `Parameter` to `ProtocolChart.razor`.
- **Decoupling:** Filtering only affects the visual rendering in `ProtocolChart`; the `ProtocolSummary` calculation in the Application layer remains unchanged (as per requirement).

## Phase 1: UI State & Legend
- [ ] **Task 1.1: Add Visibility State to ProtocolPage**
  - Initialize a `HashSet<ProtocolEventType>` containing all types by default.
  - Add a toggle method: `ToggleVisibility(ProtocolEventType type)`.
- [ ] **Task 1.2: Update Legend with Checkboxes**
  - Modify the legend in `ProtocolPage.razor` to include `<input type="checkbox">` for each activity type.
  - Bind the checkbox state to the visibility set.
- [ ] **Task 1.3: Pass State to ProtocolChart**
  - Update `ProtocolChart.razor` to accept `HashSet<ProtocolEventType> VisibleTypes` as a parameter.
  - Pass the state from `ProtocolPage.razor`.

## Phase 2: Chart Filtering
- [ ] **Task 2.1: Implement Filtering in ProtocolChart**
  - In the event rendering loop, check if `ev.Type` is present in `VisibleTypes`.
  - Skip rendering if the type is hidden.
- [ ] **Task 2.2: Manual UI Verification**
  - Start the application.
  - Verify that all activity types are visible by default.
  - Uncheck "Sleep": verify sleep lines disappear while totals in the summary remain.
  - Uncheck other types: verify icons disappear.
  - Re-check: verify icons reappear.

## Phase 3: Final Review
- [ ] **Task 3.1: Architectural Review**
- [ ] **Task 3.2: Complete Track**
