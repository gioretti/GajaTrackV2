# Design Doc: Sleep Correlation Chart

## 1. Overview
The Sleep Correlation Chart is a scatter plot designed to visualize the potential relationship between daytime nap duration and the frequency of night wakings. This helps parents identify patterns, such as whether longer naps correlate with more disrupted nights.

## 2. Component Design
- **File:** `src/GajaTrack.Presentation/GajaTrack.WebApp/Components/Pages/DailyRhythmMap/SleepCorrelationChart.razor`
- **Input:** `List<DailyRhythmMapDay> Days`
- **Implementation:** Native SVG rendering.

### Data Mapping
- **X-Axis:** Daytime Sleep (Naps). Range: 0 to 12 hours. Input data is `Summary.NapsMinutes`.
- **Y-Axis:** Night Wakings. Range: 0 to maximum found in the 30-day window (minimum 5). Input data is `Summary.NightWakingCount`.
- **Plot Point:** A `<circle>` for each day.

## 3. Visual Specifications
- **Dimensions:** Matches sibling charts (`TotalWidth: 1600`, `TotalHeight: 350`).
- **Styling:**
    - Circles: Diameter 8px, Blue fill (`#1a237e`) with 50% opacity to reveal overlapping points.
    - Grid: Vertical lines every hour (X), horizontal lines for every waking count (Y).
    - Labels: "Nap Duration (Hours)" on X-axis, "Night Wakings" on Y-axis.
- **Interactivity:** SVG `<title>` tooltip showing the date, exact nap duration, and waking count.

## 4. Integration
- The component will be added to `SleepTrendPage.razor` as the third chart in the vertical stack.
- It will consume the same `_days` list (30 days) as the other charts on the page.

## 5. Verification Plan
- **Automated:** Verify component rendering with varying data sets (no data, high wake count, zero nap days).
- **Manual:** Verify that points correctly align with the grid and tooltips display correct local dates.
