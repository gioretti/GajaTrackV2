# Implementation Plan: Decouple Frontend and API

## Phase 1: Frontend Autonomy
1. [ ] Move `app.css`, `export.js` and other assets from `Web/wwwroot` to `Web.Client/wwwroot`.
2. [ ] Create `index.html` in `Web.Client/wwwroot`.
3. [ ] Update `Web.Client/Program.cs` to ensure it boots as a standalone app.

## Phase 2: Backend Purification
1. [ ] Delete `Components/App.razor`, `Components/Error.razor`, `Components/_Imports.razor` from `Web`.
2. [ ] Update `Web/Program.cs`:
    - Remove `AddRazorComponents()`.
    - Remove `MapRazorComponents<App>()`.
    - Configure static file serving to point to the Blazor WASM files.
3. [ ] Remove `wwwroot` from `Web`.

## Phase 3: Verification
1. [ ] `dotnet build`
2. [ ] `dotnet test`
3. [ ] Manual verify of the SPA loading and fetching data.
