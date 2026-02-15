# Implementation Plan: Project Renaming & Realignment

## Phase 1: Physical Renaming
1. [ ] Rename `src/GajaTrack.Web/GajaTrack.Web.Client` to `src/GajaTrack.Web/GajaTrack.WebApp`.
2. [ ] Rename `src/GajaTrack.Web/GajaTrack.Web` to `src/GajaTrack.Web/GajaTrack.RestApi`.
3. [ ] Rename `.csproj` files to match.
4. [ ] Rename parent folder `src/GajaTrack.Web` to `src/GajaTrack.Presentation`.

## Phase 2: Solution & Code Updates
1. [ ] Update `GajaTrack.sln` to reflect new paths and names.
2. [ ] Global search and replace namespaces:
    - `GajaTrack.Web.Client` -> `GajaTrack.WebApp`
    - `GajaTrack.Web` -> `GajaTrack.RestApi`
3. [ ] Update project references in `.csproj` files.
4. [ ] Update `_Imports.razor` and `Program.cs` files in both projects.

## Phase 3: Integration & Documentation
1. [ ] Update test projects that reference the Web/Client projects.
2. [ ] Update `GEMINI.md` repository map.
3. [ ] Final build and manual UI verification.
