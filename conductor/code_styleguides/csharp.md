# Baby Gaja Coding Guidelines (C#)

## 1. Naming Conventions
- **PascalCase:** For class names, method names, constants, properties, and namespaces.
- **camelCase:** For local variables and parameters.
- **_camelCase:** For private fields (leading underscore).
- **Interfaces:** Prefix with `I` (e.g., `ITrackingRepository`).
- **Descriptive Naming:** Avoid generic prefixes like `Data` or `Dto`. Use names that reflect the intent and context (e.g., `BottleFeedImportRequest` or `SleepSessionSummary`).
- **Booleans:** Prefix with `Is`, `Has`, `Can`, etc.

## 2. Formatting (Codebase Consistency)
- **Indentation:** 4 spaces.
- **Braces:** Allman style (Opening brace `{` on a new line).
- **Namespaces & Organization:** Use file-scoped namespaces. The folder structure MUST exactly match the namespace structure (e.g., namespace `GajaTrack.Domain.Entities` MUST be in `Domain/Entities/` folder relative to the project root).
- **Explicit Access:** Always declare access modifiers.

## 3. The "Keep It Simple" (KISS) Principle
- **Zero Dead Code:** No unused variables, methods, or unreachable logic.
- **No Over-engineering:** Implement only what is required for the current track.
- **Cleanliness:** No commented-out code or placeholder comments.
- **Test-Only Noise:** Avoid logic or public members used solely for testing.

## 4. Architectural Integrity
- **Boundary Enforcement:** Respect Domain, Application, and Infrastructure layers.
- **Dependency Rule:** Dependencies MUST point inwards towards the Domain.
- **Cross-Reference:** Always verify implementation against existing **ADRs** in `docs/adr/`.

## 5. Testing Standards
- **Naming:** Unit test classes MUST be named after the class being tested with a "Test" postfix (e.g., `BottleFeedTest`, NOT `BottleFeedTests`).
- **Namespaces:** Test class namespaces MUST exactly match the namespace of the class being tested. This improves discoverability and boundary enforcement.
- **Separation:**
    - `GajaTrack.Test`: For pure logic unit tests (no DB, no external dependencies).
    - `GajaTrack.IntegrationTest`: For tests requiring infrastructure, database, or API hosting.

## 6. Review Standards
- **Naming Challenge:** Names must be descriptive and follow conventions.
- **Complexity:** Methods should follow the Single Responsibility Principle.
