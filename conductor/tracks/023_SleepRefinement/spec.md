# Specification: Sleep Categorization Refinement

## Problem Statement
The initial implementation of sleep categorization used Tuples, hardcoded values, and redundant DTO properties, which violates our project's standards for clarity and maintainability.

## Goals
- Address all architectural and code quality feedback from PR #7.
- Enforce new coding guidelines for Constants and Type Safety.

## Acceptance Criteria
1. `CalculateSleep` returns a `Result` record instead of a Tuple.
2. Hardcoded hours (06:00, 18:00) are moved to `BabyDay` as named constants.
3. `TotalSleepMinutes` is removed from DTOs (calculated in UI).
4. Tests use explicit and descriptive names.
5. Project styleguide updated.
