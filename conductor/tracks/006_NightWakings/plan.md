# Plan: Night Wakings Metric

Implement the night wakings count in the daily summary, defined as any sleep session ending between 18:00 and 06:00 (exclusive of the 06:00 wake-up).

## Architectural Strategy
- **Application Layer:** Update `ProtocolSummary` DTO to include the waking count. Update `ProtocolService` to calculate the count.
- **UI Layer:** Update `ProtocolSummaryCell.razor` to render the new metric.

## Phase 1: Application Logic (TDD)
- [x] **Task 1.1: Update DTO**
  - Add `NightWakingCount` to `ProtocolSummary` in `ProtocolDtos.cs`.
- [ ] **Task 1.2: Write Failing Unit Tests (TDD)**
  - Add test cases to `ProtocolServiceTests.cs`:
    - Case: Single sleep session ending at 05:45 (Count: 0).
    - Case: Two sleep sessions, ending at 02:00 and 05:45 (Count: 1).
    - Case: Sleep session spanning past 06:00 (Count: 0).
    - Case: No sleep sessions (Count: 0).
- [ ] **Task 1.3: Implement Calculation**
  - Update `ProtocolService.cs` to calculate `NightWakingCount`.
  - Logic: Identify all `ProtocolEventType.Sleep` events in `dayEvents`. If count > 1, the number of night wakings is `(number of sleep sessions ending before 06:00) - 1`. If 1 session ends before 06:00, count is 0. If sessions end after 06:00, they don't contribute to the "interruption" count.
  - Specifically: `dayEvents.Where(e => e.Type == Sleep && e.End < 06:00).Count() - 1` (clamped to 0).

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
