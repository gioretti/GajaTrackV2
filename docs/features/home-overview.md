# Home Overview / Stats

## Overview
The Home Overview serves as the primary dashboard when users open the GajaTrack application. Alongside providing a quick link to export data, it displays high-level, all-time tracking statistics for the baby.

## Implementation Details

### User Interface (`Home.razor`)
- Displays five key metric cards (Nursing, Bottle, Sleep, Diapers, Crying).
- Fetches data asynchronously on page load via the `StatsApiClient`.
- Displays a loading spinner while the network request is inflight.

### Application Logic (`StatsService.cs`)
- **API Endpoint:** Uses `GET /api/stats`.
- **Querying:** The backend implementation uses Entity Framework Core's `CountAsync` method directly on the `DbSet` for each tracking entity.
- **Scope:** The stats represent the **all-time total count** of records inserted into the database. They do not currently filter by day, week, or active timeframe.
