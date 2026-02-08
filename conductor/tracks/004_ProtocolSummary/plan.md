# Plan: Protocol Daily Summary Column

This plan implements the Daily Summary Column for the 24-hour protocol, starting with the total sleep duration calculation strictly aligned with the 06:00 - 06:00 protocol day.

## Architectural Notes
- **Domain Layer:** Add a `TotalSleepMinutes` property to `ProtocolDay` (or a dedicated `ProtocolSummary` DTO) to encapsulate the calculation.
- **Application Layer:** Update `ProtocolService` to populate the summary data.
- **UI Layer:** Update the Blazor components to render the new column and its extensible container.

## Phase 1: Application Logic & DTOs (TDD)
- [x] **Task 1.1: Update DTOs (Structural Prep)**
  - Add `ProtocolSummary` record to `ProtocolDtos.cs`.
  - Update `ProtocolDay` to include a `Summary` property (nullable/empty initially).
- [x] **Task 1.2: Write Failing Unit Tests (TDD Start)**
  - Create/Update tests in `ProtocolServiceTests` to assert that `TotalSleepMinutes` is correctly calculated for a `ProtocolDay`.
  - Include scenarios for:
    - No sleep events (0 mins).
    - Multiple sleep events within the day.
    - Sleep events spanning across the 06:00 boundary (verifying only the clipped portion is summed).
  - **Status:** Tests should fail (logic not implemented).
- [x] **Task 1.3: Implement Calculation Logic**
  - Update `ProtocolService.GetProtocolAsync` to populate the `ProtocolSummary` by summing the `DurationMinutes` of all `Sleep` events.
  - **Status:** Tests should now pass.

## Phase 2: UI Implementation
- [x] **Task 2.1: Update Protocol Grid Layout**
  - Modify `ProtocolChart.razor` (or the container component) to include a new column on the right.
  - Ensure the layout remains responsive/mobile-friendly (horizontal scroll for the grid is already allowed).
- [x] **Task 2.2: Implement Summary Container Component**
  - Create a small sub-component or styled `<div>` for the summary cell.
  - Use Flexbox `column` layout to ensure future metrics can be stacked vertically (Extensibility requirement).
- [x] **Task 2.3: Render Sleep Total**
  - Display the formatted sleep duration (e.g., "14h 30m").
- [>] **Task 2.4: Manual UI Verification**
  - Start the application.
  - Navigate to the protocol page.
  - Verify that the summary column is visible and correctly aligned with the rows.
  - Verify that the totals match the visual sleep bars.

## Phase 3: Final Review
- [ ] **Task 3.1: Architectural Review**
  - Submit Implementation Summary to Architect.
- [ ] **Task 3.2: Complete Track**
  - Update `conductor/tracks.md`.
