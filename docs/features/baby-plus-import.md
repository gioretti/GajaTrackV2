# Baby+ Data Import

## Overview
The Baby+ Data Import feature allows users to migrate their historical tracking data from the Philips Baby+ app (iOS or Android) directly into GajaTrack. The process supports importing JSON exports of Nursing, Bottle Feeding, Sleep, Diaper Changes, and Crying Sessions.

## Implementation Details

### User Interface (`BabyPlusImport.razor`)
- **File Upload:** Accepts `.json` export files with a maximum enforced file size of 10MB to prevent memory exhaustion on the Blazor WebAssembly client.
- **API Communication:** Upon selection, the file stream is posted securely to the REST API via `HttpClient` to `/api/import/babyplus`.
- **User Feedback:** The UI displays a processing spinner during the upload/import phase and provides a detailed summary of the total imported records per category upon success.

### Application Logic (`BabyPlusImportService.cs`)
- **Deserialization:** Handles the specific JSON schema of the Baby+ export (`BabyPlusExport` DTO). It utilizes custom JSON converters (`PolymorphicDateTimeConverter`, `PolymorphicBoolConverter`, etc.) to gracefully handle data inconsistencies common in raw JSON exports.
- **Idempotency:** To prevent data duplication during multiple or overlapping imports, the service maps the Baby+ JSON `Pk` field to an `ExternalId` on the domain entities. Existing records are loaded into memory first and compared against incoming records.
- **Optimized Persistence:** 
    - Entity Framework Core tracking (`AutoDetectChangesEnabled`) is disabled to maximize throughput and minimize memory footprint.
    - Records are inserted via `AddRange` in batches of 500 to optimize SQLite write performance without causing locking scenarios.

## Mapped Entities
- **Nursing:** Maps to `NursingFeed` (Start/End times).
- **Bottle Feeding:** Maps to `BottleFeed` (Date, Amount, Formula flag).
- **Sleep:** Maps to `SleepSession` (Start/End times).
- **Diapers:** Maps to `DiaperChange` (Date, Type).
- **Crying:** Maps to `CryingSession` (Start/End times).
