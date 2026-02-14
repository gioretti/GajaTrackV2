# Implementation Plan: Separate Test Projects

## Phase 1: Scaffolding
1. [ ] Create `tests/GajaTrack.Tests.Unit` project.
2. [ ] Create `tests/GajaTrack.Tests.Integration` project.
3. [ ] Add both to the solution.

## Phase 2: Migration
1. [ ] Move `Unit/` contents to `GajaTrack.Tests.Unit`.
2. [ ] Move `Integration/` and `Fixtures/` contents to `GajaTrack.Tests.Integration`.
3. [ ] Configure `.csproj` files with correct references.

## Phase 3: Verification & Cleanup
1. [ ] Run all tests.
2. [ ] Delete the old `GajaTrack.Tests` folder.
3. [ ] Remove the old project from the solution.
