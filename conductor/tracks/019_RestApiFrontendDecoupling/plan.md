# Implementation Plan: REST API Frontend Decoupling

## Structural Design
The solution currently has a `GajaTrack.Web` (Host) and `GajaTrack.Web.Client` (WASM).
We will leverage `System.Net.Http.Json` for simple, type-safe API calls.

## Phase 1: Dependency Injection & Infrastructure
1. [ ] Update `GajaTrack.Web.Client/Program.cs`:
    - Ensure `HttpClient` is registered with the correct `BaseAddress`.
    - Remove any direct registration of Application services (`IDailyRhythmMapService`, etc.) from the Client project.
2. [ ] Verify that DTOs used in the API are in a shared project or accessible to both `Web.Client` and `Application`. (Currently they are in `GajaTrack.Application`, which is okay as long as `Web.Client` references it, but we should eventually consider a separate `Contracts` project if we want zero logic in frontend).

## Phase 2: Refactor Components
1. [ ] **DailyRhythmMapPage.razor**:
    - Replace `@inject IDailyRhythmMapService` with `@inject HttpClient Http`.
    - Refactor `LoadData` to use `Http.GetFromJsonAsync<List<DailyRhythmMapDay>>`.
2. [ ] **Home.razor**:
    - Replace direct service injection with `HttpClient`.
    - Fetch stats from `/api/stats`.
3. [ ] **BabyPlusImport.razor**:
    - Refactor file upload logic to use `Http.PostAsync` to `/api/import/babyplus`.

## Phase 3: Verification
1. [ ] `dotnet build` to ensure no project reference violations.
2. [ ] Manual verify:
    - Navigation between pages.
    - Data loading on Daily Rhythm Map.
    - Stats loading on Home.
    - File upload functionality.
3. [ ] Check Network tab in browser to confirm all calls are hitting `/api/`.
