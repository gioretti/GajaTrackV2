# Sleep Analysis

## Overview
The Sleep Analysis feature provides longitudinal insights into the baby's sleep patterns. It is accessible via the "Sleep Trends" page and aggregates data from the `DailyRhythmMap` endpoint over a 30-day window (by default) to render various visual representations of sleep duration and quality.

## Implementation Details

### Data Sourcing
The Sleep Analysis features do not have a dedicated API endpoint. Instead, the `SleepTrendPage.razor` fetches the same list of `DailyRhythmMapDay` objects (via `/api/daily-rhythm-map`) used by the core Daily Rhythm map, but specifies a wider default date range (`-29` days to today).

### Visualizations

The data is rendered completely client-side via custom SVG components in Blazor WebAssembly.

1. **Sleep Duration Trend (`SleepTrendChart.razor`)**
   - An area chart that visualizes total sleep over time.
   - Shows stacked areas comparing **Night Sleep** (dark blue) vs. **Naps** (yellow).
   - Allows parents to see how total daily sleep fluctuates and how the distribution shifts between daytime and nighttime.

2. **Sleep Duration Line Chart (`SleepDurationLineChart.razor`)**
   - An alternative line-based visualization focusing specifically on the continuous trend of total sleep durations.

3. **Nap Duration vs. Night Wakings (`SleepCorrelationChart.razor`)**
   - A scatter plot designed to uncover correlations between daytime napping and nighttime disruptions.
   - **X-Axis:** Total daytime nap duration (hours).
   - **Y-Axis:** Number of night wakings. *(Note: Evaluated via client-side logic filtering for events > 20 mins).*

4. **Nap Duration vs. Avg Night Session Length (`SleepSessionDurationCorrelationChart.razor`)**
   - A scatter plot assessing if longer daytime naps lead to shorter or longer consolidated night sleep sessions.
   - **X-Axis:** Total daytime nap duration (hours).
   - **Y-Axis:** Average duration of nighttime sleep sessions. *(Note: Evaluated via client-side logic filtering for sessions > 20 mins).*

