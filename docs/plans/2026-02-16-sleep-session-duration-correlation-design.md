# Design Doc: Sleep Session Duration Correlation Chart

## 1. Overview
This scatter plot visualizes the relationship between daytime nap duration and the average duration of individual night sleep sessions. It aims to help identify if more daytime sleep leads to shorter, more fragmented sleep sessions at night.

## 2. Component Design
- **File:** `src/GajaTrack.Presentation/GajaTrack.WebApp/Components/Pages/DailyRhythmMap/SleepSessionDurationCorrelationChart.razor`
- **Input:** `List<DailyRhythmMapDay> Days`
- **Logic:** Approach 2 (Frontend Calculation)
    - Filter `day.Events` for `Type == Sleep` where `StartMinute >= 720` (18:00 threshold).
    - Skip days with zero matching sessions.
    - Calculate average duration: `TotalNightSessionMinutes / SessionCount`.

### Data Mapping
- **X-Axis:** Daytime Nap Duration (Hours). Range: 0 to 12 hours. Input: `Summary.NapsMinutes`.
- **Y-Axis:** Average Night Session Duration (Hours). Range: 0 to maximum found in data (minimum 6h).
- **Plot Point:** A `<circle>` for each valid day.

## 3. Visual Specifications
- **Dimensions:** `TotalWidth: 1600`, `TotalHeight: 350` (Matches sibling charts).
- **Styling:**
    - Circles: Orange fill (`#ffb300`) with 50% opacity.
    - Grid: Vertical lines every hour (X), horizontal lines every hour (Y).
    - Labels: "Nap Duration (Hours)" on X-axis, "Avg Night Session (Hours)" on Y-axis.
- **Interactivity:** SVG `<title>` tooltip showing the date, nap duration, and average session length.

## 4. Integration
- Add to `SleepTrendPage.razor` as the fourth chart in the stack.

## 5. Verification Plan
- **Automated:** Build check.
- **Manual:** Verify correct filtering of sessions (starting after 18:00) and accurate average calculation via tooltip inspection.
