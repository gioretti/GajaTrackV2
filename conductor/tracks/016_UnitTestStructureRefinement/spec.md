# Specification: Unit Test Structure Refinement

## Goal
Standardize unit test naming and namespaces to improve discoverability and maintain a clean project structure.

## Requirements
- **Class Naming:** Test classes must be named `<SubjectClass>Test` (e.g., `BottleFeedTest`).
- **Namespaces:** Test class namespaces must exactly match the namespace of the class being tested.
- **Guidelines:** Add these rules to the project's coding style guidelines.

## Acceptance Criteria
- [ ] All unit test classes in `GajaTrack.Test` follow the `<SubjectClass>Test` pattern.
- [ ] All unit test namespaces match their subject classes.
- [ ] Coding guidelines updated in `conductor/code_styleguides/csharp.md`.
- [ ] All 69 tests pass.
