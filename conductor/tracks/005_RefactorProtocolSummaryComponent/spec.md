# Specification: Refactor Protocol Summary to Standalone Component

## Context
The daily summary logic is currently embedded directly within `ProtocolChart.razor`. To support future complexity and maintain clean separation of concerns, this should be refactored into a standalone component.

## Goal
Extract the summary rendering logic into a dedicated Blazor component to improve maintainability and support extensibility for future metrics.

## Acceptance Criteria
1. **Extraction:** Create `ProtocolSummaryCell.razor` in the same directory as `ProtocolChart.razor`.
2. **Component Interface:** The new component must accept a `ProtocolSummary` object as a parameter.
3. **SVG Integration:** Since the summary is part of the SVG grid, the component should render SVG elements (likely wrapped in a `<g>` tag).
4. **Behavioral Parity:** The visual output and layout position must remain identical to the current implementation.
5. **Cleanliness:** Remove all summary formatting and layout logic from `ProtocolChart.razor`.
