# Specification: Daily Rhythm Map UI Refinement

## Problem Statement
The current `RowHeight` of 75px makes the Daily Rhythm Map chart vertically sparse, requiring excessive scrolling. We need to reduce the row height to restore a compact view without causing the summary information to overlap or overflow.

## Goals
- Reduce `RowHeight` in the Daily Rhythm Map.
- Refactor the Summary layout to be more compact.
- Maintain clarity and legibility of all data.

## Acceptance Criteria

### 1. Row Dimensions
- Reduce `RowHeight` from 45px to approximately **25px - 30px**.

### 2. Icon Scaling
- Icons (Nursing triangles, Bottles, Diapers) must be scaled up to occupy approximately **80%** of the row height.

### 3. Summary Layout (Ultra Compact)
- The summary will use a single horizontal row or highly optimized two-column layout to fit the 30px height.

### 3. Visual Polish
- Ensure font sizes and alignments are adjusted for the smaller vertical space.
- Date labels (Left) should remain centered vertically in the smaller rows.
