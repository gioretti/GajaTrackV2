# Daily Rhythm Map

## Overview
The Daily Rhythm Map (formerly known as the "24-Hour Protocol") is the core visualization feature of GajaTrack. It displays a contiguous sequence of days (from 06:00 to 06:00 local time) and maps all tracking activities chronologically on a horizontal timeline. This allows parents to intuitively understand their baby's daily flow, sleep patterns, and feeding routines.

## Implementation Details

### User Interface (`DailyRhythmMapPage.razor` & `DailyRhythmMapChart.razor`)
- **WebAssembly Client:** The UI relies on Blazor WebAssembly, ensuring responsive rendering of complex DOM elements (hundreds of events mapped across days).
- **Date Range Selection:** Users can select a Start and End date. The UI requests data via `DailyRhythmMapApiClient` which hits the `REST API`.
- **Visibility Toggles:** Users can toggle the visibility of specific event types (Sleep, Crying, Nursing, Formula, Milk, Diaper) through checkboxes. The UI uses an `in-memory` HashSet (`_visibleTypes`) to instantly filter the SVG chart without refetching data.
- **Legends & Styling:** 
  - Sleep: Gray bar
  - Crying: Wavy line
  - Nursing: White triangle
  - Formula: Blue triangle
  - Milk: Gray triangle
  - Diaper: Yellow/Brown rectangles (Wet/Mixed)

### Application & Domain Logic (`DailyRhythmMapService.cs`)
- **Domain Orchestration:** The service delegates strict boundary calculations to the domain layer (e.g. `GetBabyDay.Execution`, `CalculateSleep`, `CountWakings`).
- **Data Mapping & Intersection Logic:**
  - The application queries the database via the `GetBabyDay` pattern.
  - **Intervals (Sleep, Crying):** The service computes the `TimeRange` of the event. If an event crosses a day boundary (e.g., sleep from 05:00 to 07:00), the `GetIntersection` method clips it so only the portion within that specific BabyDay (06:00 to 06:00) is returned for that day's UI list.
  - **Points (Nursing, Bottle, Diaper):** Mapped strictly to their specific timestamp.
- **Summaries:** Each day includes a localized sum of `NapsMinutes`, `NightSleepMinutes`, and `NightWakings`. The definition of "Night" versus "Day" is encapsulated strictly in the `BabyDay` domain entity (`BabyDay.NightTimeStart`, `BabyDay.DayTimeStart`).
