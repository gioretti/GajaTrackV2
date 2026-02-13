# Implementation Plan: Global Interactivity & Component Migration

## Phase 1: Migration
- [x] Create `Layout` folder in `Web.Client`.
- [x] Move `MainLayout.razor` and `MainLayout.razor.css`.
- [x] Move `Routes.razor` to `Web.Client`.

## Phase 2: Configuration
- [x] Update `_Imports.razor` in both projects for new namespaces.
- [x] Update `App.razor` to apply `@rendermode="InteractiveAuto"` to `HeadOutlet` and `Routes`.
- [x] Remove `@rendermode` from `Home.razor`, `DailyRhythmMapPage.razor`, and `BabyPlusImport.razor`.

## Phase 3: Verification
- [x] `dotnet build`
- [x] `dotnet test` (69/69 passing)
- [x] Manual runtime verification of `/api/stats` and homepage rendering.
