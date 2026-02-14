# Specification: Test Project Refinement

## Goal
Align test project naming with user preference and clean up obsolete project references.

## Requirements
- **Rename Unit Project:** `GajaTrack.Tests.Unit` -> `GajaTrack.Test`.
- **Rename Integration Project:** `GajaTrack.Tests.Integration` -> `GajaTrack.IntegrationTest`.
- **Clean AssemblyInfo:** Remove `InternalsVisibleTo("GajaTrack.Tests")` from `src/GajaTrack.Infrastructure/AssemblyInfo.cs`.
- **Namespace Update:** Update namespaces in test files to match the new project names.

## Acceptance Criteria
- [ ] Solution builds successfully.
- [ ] All 69 tests pass.
- [ ] Project names are `GajaTrack.Test` and `GajaTrack.IntegrationTest`.
