# Story: Export Data as JSON
status: Closed

## Description
As a user who wants to backup or analyze my data,
I want to export all my tracking records as a JSON file,
So that I can use it in other tools or keep a personal archive.

## Technical Details
- **Format**: JSON
- **Filename**: `gajatrack_export_YYYY-MM-DD.json`
- **Root Object**: `GajaTracking`
- **Structure**:
  - `NursingFeeds`: List of nursing records.
  - `BottleFeeds`: List of bottle records.
  - `SleepSessions`: List of sleep records.
  - `DiaperChanges`: List of diaper records.
- **Data Types**:
  - **IDs**: Internal `Id` (Guid) only. **Do not expose `ExternalId`**.
  - **Dates**: ISO 8601 Strings (UTC).
  - **Enums**: String representation (e.g., "Wet", "Formula").

## Gherkin Scenarios

### Scenario: Export all data to JSON
Given the database contains tracking records
When I click the "Export Data" button on the Home page
Then the system should generate a JSON file
And the file root must be "GajaTracking"
And all dates must be in ISO 8601 format
And all Enums must be serialized as strings
And the browser should trigger a download for `gajatrack_export_[DATE].json`

## Acceptance Criteria
- [ ] Export button is available on the Home page near the dashboard.
- [ ] Clicking export triggers a JSON file download.
- [ ] The file structure matches the "GajaTracking" root requirement.
- [ ] No `ExternalId` is present in the exported data.
- [ ] Code follows the simple approach (In-memory serialization for MVP).

## Technical Analysis

### Domain & DTOs
We need a clean set of DTOs for export to ensure the shape matches the requirements without affecting the internal Domain entities.

```csharp
public record GajaTrackExport(
    List<ExportNursingFeed> NursingFeeds,
    List<ExportBottleFeed> BottleFeeds,
    List<ExportSleepSession> SleepSessions,
    List<ExportDiaperChange> DiaperChanges
);

public record ExportNursingFeed(Guid Id, DateTime StartTime, DateTime? EndTime);
public record ExportBottleFeed(Guid Id, DateTime Time, int AmountMl, string Content);
public record ExportSleepSession(Guid Id, DateTime StartTime, DateTime? EndTime);
public record ExportDiaperChange(Guid Id, DateTime Time, string Type);
```

### Infrastructure
- **Service**: `IExportService` / `ExportService`.
- **Implementation**:
  - Fetch all data (AsNoTracking).
  - Map to DTOs.
  - Serialize using `System.Text.Json` with `JsonStringEnumConverter`.
  - Return `byte[]` or `MemoryStream`.

### UI (Blazor Download)
- **Problem**: Blazor Server cannot "return" a file result like MVC.
- **Solution**:
  1. Service returns `byte[]`.
  2. Component calls JS function `downloadFileFromStream`.
  3. `script.js` creates a Blob URL and clicks it.

### Implementation Plan
1.  **Infrastructure**: Implement `ExportService`.
2.  **Web**: Add `export.js` for download handling.
3.  **UI**: Add Export button to `Home.razor` and wire up the call.
4.  **Tests**: Unit test the serialization format.