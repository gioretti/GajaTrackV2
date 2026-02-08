# Plan: Night Wakings Metric

Implement the night wakings count in the daily summary, defined as any sleep session ending between 18:00 and 06:00 (exclusive of the 06:00 wake-up).

## Architectural Strategy
- **Application Layer:** Update `ProtocolSummary` DTO to include the waking count. Update `ProtocolService` to calculate the count.
- **UI Layer:** Update `ProtocolSummaryCell.razor` to render the new metric.

## Phase 1: Application Logic (TDD)
- [ ] **Task 1.1: Update DTO**
  - Add `NightWakingCount` to `ProtocolSummary` in `ProtocolDtos.cs`.
- [ ] **Task 1.2: Write Failing Unit Tests (TDD)**
  - Add test cases to `ProtocolServiceTests.cs`:
    - Count wakings where `EndMinute` is between 720 (18:00) and 1440 (06:00).
    - Ensure a session ending exactly at 1440 (06:00) is **not** counted.
    - Ensure multiple wakings are counted correctly.
- [ ] **Task 1.3: Implement Calculation**
  - Update `ProtocolService.cs` to calculate `NightWakingCount`.
  - Logic: Count `ProtocolEventType.Sleep` events in `dayEvents` where `StartMinute + DurationMinutes` is within the night window [720, 1440).

## Phase 2: UI Implementation
- [ ] **Task 2.1: Update ProtocolSummaryCell.razor**
  - Add a new `<text>` element to display "Night Wakings (18-06): [Count]".
  - Position it below the "Sleep" total.
- [ ] **Task 2.2: Manual UI Verification**
  - Launch application and verify the metric appears correctly for days with night wakings.
  - Verify alignment and label clarity.

## Phase 3: Finalization
- [ ] **Task 3.1: Architectural Review**
- [ ] **Task 3.2: Complete Track**
