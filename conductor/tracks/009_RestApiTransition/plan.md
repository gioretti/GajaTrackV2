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
- [>] Move `BabyPlusImport.razor` to `GajaTrack.Web.Client`
- [ ] Update UI to use the new API
- [ ] Verify import functionality via UI

## Phase 3: Cleanup
- [ ] Remove redundant server-side-only views
- [ ] Consolidate shared logic
