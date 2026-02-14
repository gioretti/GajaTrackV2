# Specification: Separate Test Projects

## Goal
Improve developer productivity and project clarity by separating fast unit tests from slower integration tests.

## Requirements
- **Unit Project (`GajaTrack.Tests.Unit`):**
    - Fast, zero-dependency (other than Domain/Application).
    - Contains all tests currently in the `Unit/` folder.
- **Integration Project (`GajaTrack.Tests.Integration`):**
    - Slower, handles DB and Web setup.
    - Contains all tests currently in the `Integration/` folder.
    - Owns the `Fixtures/` directory.
- **Cleanup:** Remove the old unified `GajaTrack.Tests` project.

## Acceptance Criteria
- [ ] Both test projects build and run correctly.
- [ ] 69/69 tests still pass across both projects.
- [ ] The solution structure is updated.
