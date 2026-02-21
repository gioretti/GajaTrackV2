# Data Export

## Overview
The Data Export feature provides a way for users to extract their tracked records from GajaTrack into a JSON file for backup or external analysis.

## Implementation Details

### User Interface (`Home.razor`)
- The export functionality is accessible via a button on the Home dashboard.
- Clicking the export button calls the `ExportApiClient` which triggers a file download in the user's browser via Blazor JSInterop.

### Application Logic (`ExportService.cs` & `ExportApiClient.cs`)
- **API Endpoint:** Uses `GET /api/export` to request the data.
- **Data Aggregation:** `ExportService.cs` retrieves records from the SQLite database using Entity Framework `AsNoTracking()` for optimal read performance.
- **Serialization:** Combines the pulled entities into `GajaTrackExport` DTOs and serializes them into a formatted (indented, camelCase) UTF-8 JSON byte array.

## Exported Entities
- **Nursing Feeds**
- **Bottle Feeds**
- **Sleep Sessions**
- **Diaper Changes**

*(Note: Crying Sessions are currently excluded from the export.)*
