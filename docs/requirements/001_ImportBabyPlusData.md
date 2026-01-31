# Story: Import Philips Baby+ Data
status: Closed

## Description
As a parent transitioning from Philips Baby+,
I want to import my existing Feeding (Milk only), Sleep, and Diaper data,
So that I can continue tracking these core metrics without losing history.

## Technical Details
- **Source Format**: JSON (`babyplus_data_export.json`)
- **Data Mapping Requirements**:
  - **Nursing (`baby_nursingfeed`)**: `startDate`, `endDate`.
  - **Bottle (`baby_bottlefeed`)**: `date`, `amountML`, `isFormula`.
  - **Sleep (`baby_sleep`)**: `startDate`, `endDate`.
  - **Nappy (`baby_nappy`)**: `date`, `type`.
  - **Deduplication**: Use `pk` from JSON as the external reference to prevent duplicate imports.

## Scope**:
- **Feeding**: Nursing and Bottle only.
- **Sleep**: Start and End times.
- **Diapers**: Date and Type.
- **Out of Scope**:
  - **Solids**: `baby_solidfeed`.
  - Side-specific nursing durations (Left/Right).
  - Growth, Health, Media.

## Assumption**:
- **Single Baby System**: The system supports only one baby for this MVP.
- **Import Strategy**: All records assigned to the single "Default Baby" (Fixed ID). 

## Gherkin Scenarios

### Scenario: Import Core Tracking Data for Default Baby
Given I have a valid export file "babyplus_data_export.json"
When I upload the file to GajaTrack
Then the system should:
1. Import all Nursing and Bottle feed records.
2. Import all Sleep sessions.
3. Import all Diaper changes.
   And the imported values must be transformed correctly:
- Unix timestamps (double) converted to local/UTC DateTime.
- `isFormula` (0/1) mapped to `BottleContent` Enum.
- `type` string (Wet/Mixed) mapped to `DiaperType` Enum.
  And I should see a summary count for Feeds (Milk), Sleep, and Diapers.

### Scenario: Import Android-formatted Export Data
Given I have a valid export file "babyplus_data_export_android.json"
And this file uses ISO 8601 strings for dates and native JSON booleans
When I upload the file to GajaTrack
Then the system should:
1. Detect and parse ISO 8601 date strings correctly.
2. Parse native JSON boolean values correctly.
3. Import all records as if they were in the iOS format.
   And the data integrity must be maintained (e.g., correct UTC conversion).

### Scenario: Idempotent Import
Given I have already imported "babyplus_data_export.json"
When I upload the same file again
Then the system should skip records with existing `pk` values.

### Scenario: Strict Validation Failure (Sad Path)
Given I have an export file where one Nursing record has an EndDate before a StartDate
When I upload the file to GajaTrack
Then the system should:
1. Abort the entire import process.
2. Roll back any database changes (Atomicity).
3. Display a detailed error message including the `pk` and the specific reason.
   And no new records should exist in the database.

## Technical Analysis

### Domain Model Design

#### 1. NursingFeed (Aggregate Root)
- `Guid Id`
- `Guid BabyId`
- `string ExternalId` (Unique Index, map to `pk`)
- `DateTime StartTime`
- `DateTime EndTime`

#### 2. BottleFeed (Aggregate Root)
- `Guid Id`
- `Guid BabyId`
- `string ExternalId` (Unique Index, map to `pk`)
- `DateTime FeedTime`
- `int AmountMl`
- `BottleContent Content` (Enum: BreastMilk, Formula)

#### 3. SleepSession (Aggregate Root)
- `Guid Id`
- `Guid BabyId`
- `string ExternalId` (Unique Index, map to `pk`)
- `DateTime StartTime`
- `DateTime EndTime`

#### 4. DiaperChange (Aggregate Root)
- `Guid Id`
- `Guid BabyId`
- `string ExternalId` (Unique Index, map to `pk`)
- `DateTime Time`
- `DiaperType Type` (Enum: Wet, Soiled, Mixed)

### Infrastructure Design
- **Persistence**: EF Core with SQLite.
- **Import Parser**: System.Text.Json.
- **Idempotency**: `HasIndex(x => x.ExternalId).IsUnique()` configuration.

## Implementation Plan

### Phase 1: Infrastructure & Domain
- [ ] Upgrade all projects to .NET 9.0.
- [ ] Create `BottleContent` and `DiaperType` Enums.
- [ ] Implement `NursingFeed`, `BottleFeed`, `SleepSession`, `DiaperChange` entities.
- [ ] Setup `GajaDbContext` with Unique Indexes on ExternalId.
- [ ] Create EF Core Migrations.

### Phase 2: Application Logic
- [ ] Implement `LegacyImportService`.
- [ ] Map JSON fields:
  - `baby_nursingfeed` -> `NursingFeed`
  - `baby_bottlefeed` -> `BottleFeed` (map `isFormula` 0/1 to Enum)
  - `baby_sleep` -> `SleepSession`
  - `baby_nappy` -> `DiaperChange` (map "Soiled", "Wet", "Mixed")
- [ ] Handle implicit "Default Baby" ID creation.

### Phase 3: Presentation
- [ ] Create `Import.razor` page.
- [ ] Increase Blazor `InputFile` max size limit to 10MB in `Program.cs` or component.
- [ ] Wire up file upload to Service.

### Phase 4: Verification
- [ ] Integration Test: Run import with `babyplus_data_export.json` and verify counts in DB.

### Validation Strategy (Strict Mode)
- **Policy**: If *any* record is invalid, the entire import transaction is rolled back.
- **Sanitization Rule**:
  - Records with `endDate : 0` (abandoned timers) are sanitized by setting `EndDate = null` before validation.
- **Polymorphic Parsing**:
  - The parser MUST handle both **Unix Timestamp** (Double, e.g., iOS) and **ISO 8601** (String, e.g
  - ., Android) formats for all date fields.
  - Boolean fields must handle `0/1` (Int) and `true/false` (Bool).
- **Rules**:
  - **NursingFeed**: `EndTime >= StartTime`.
  - **BottleFeed**: `AmountMl > 0`, Valid `BottleContent` Enum.
  - **DiaperChange**: Valid `DiaperType` Enum.
  - **General**: No future dates allowed.
- **Error Reporting**: Must throw `ImportValidationException` with details:
  - Record Type (e.g., "NursingFeed")
  - ExternalId (`pk`)
  - Specific Error (e.g., "EndTime 10:00 is before StartTime 10:30")
