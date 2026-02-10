# Plan: REST API Transition (Phase 1 & 2)

## Phase 1: Protocol Read Path

### Step 1: Protocol API Endpoint
- [x] Create integration test for `GET /api/protocol?startDate=...&endDate=...`
- [x] Implement Minimal API in `GajaTrack.Web/Program.cs`
- [x] Verify JSON serialization for `DateOnly` and `TimeSpan`
- [x] Review: Ensure it returns correctly calculated data (server-side)

### Step 2: UI Migration (Protocol View)
- [x] Move `.razor` files from `GajaTrack.Web` to `GajaTrack.Web.Client`
- [x] Update UI to call the API (direct HttpClient or wrapper)
- [x] Verify UI renders correctly in Interactive Auto mode

## Phase 2: Import Path

### Step 3: Import API Endpoint
- [x] Create integration test for `POST /api/import/babyplus`
- [x] Implement Minimal API endpoint returning count
- [x] Verify data integrity after import

### Step 4: UI Migration (Import View)
- [x] Move `BabyPlusImport.razor` to `GajaTrack.Web.Client`
- [x] Update UI to use the new API
- [x] Verify import functionality via UI (verified via integration tests)

## Phase 3: Home & Export Migration

### Step 5: Export API Endpoint
- [x] Create integration test for `GET /api/export`
- [x] Implement Minimal API endpoint returning JSON file
- [x] Verify export functionality

### Step 6: Home View Migration
- [x] Move `Home.razor` to `GajaTrack.Web.Client`
- [x] Create simple "Stats" API or update existing services to support summary data
- [x] Update UI to fetch stats via API
- [x] Update UI to use Export API

## Phase 4: Cleanup
- [x] Remove redundant server-side-only views
- [x] Consolidate shared logic
