# Specification: Daily Rhythm Map UI Refinement

## Problem Statement
The current `RowHeight` of 75px makes the Daily Rhythm Map chart vertically sparse, requiring excessive scrolling. We need to reduce the row height to restore a compact view without causing the summary information to overlap or overflow.

## Goals
- Reduce `RowHeight` in the Daily Rhythm Map.
- Refactor the Summary layout to be more compact.
- Maintain clarity and legibility of all data.

## Acceptance Criteria

### 1. Row Dimensions
- Reduce `RowHeight` from 75px to approximately **45px - 50px**.

### 2. Summary Layout (Two Columns)
- Instead of 4 rows of text, the summary will use 2 columns:
    - **Column 1:** Total Duration & Wakings.
    - **Column 2:** Naps Duration & Night Sleep Duration.
- Column 2 should be positioned to the right of Column 1 within the summary area.

### 3. Visual Polish
- Ensure font sizes and alignments are adjusted for the smaller vertical space.
- Date labels (Left) should remain centered vertically in the smaller rows.
