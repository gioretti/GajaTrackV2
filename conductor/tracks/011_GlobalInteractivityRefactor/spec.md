# Specification: Global Interactivity & Component Migration

## Context
The Blazor project was split between server-side and client-side components in a way that hindered the Single Page Application (SPA) experience. "InteractiveAuto" mode works best when the Layout and Router are shared or located in the Client project.

## Goals
- **Consolidation:** Move `MainLayout` and `Routes` from `GajaTrack.Web` to `GajaTrack.Web.Client`.
- **Interactivity:** Enable `InteractiveAuto` globally at the `App.razor` level.
- **Cleanup:** Remove per-page `@rendermode` directives.

## Acceptance Criteria
- [x] Application builds successfully.
- [x] Navigation works without full page reloads.
- [x] API calls from the client remain functional.
- [x] Server-side project only hosts entry components (`App.razor`, `Error.razor`).
