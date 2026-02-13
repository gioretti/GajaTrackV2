# Design Document: Renaming "Protocol" to "Daily Rhythm Map"

## Context
The term "Protocol" (derived from the clinical "24-Stunden-Protokoll") is deemed too rigid and formal for the Baby Gaja Tracking Application. The goal is to move towards a more human, descriptive, and visual-oriented name: **Daily Rhythm Map**.

## Goal
Perform a "Full Cleanse" refactor across the entire codebase to replace "Protocol" with "DailyRhythmMap" (or "Daily Rhythm Map" for UI labels).

## Proposed Changes

### 1. Naming Conventions
- **PascalCase (Code):** `DailyRhythmMap`
- **camelCase (Variables):** `dailyRhythmMap`
- **kebab-case (Routes/API):** `daily-rhythm-map`
- **Sentence Case (UI):** `Daily Rhythm Map`

### 2. Architectural Layers

#### Domain Layer (`src/GajaTrack.Domain`)
- `ProtocolDomainService` -> `DailyRhythmMapDomainService`
- Constants like `ProtocolDayStartHour` -> `DailyRhythmMapStartHour`

#### Application Layer (`src/GajaTrack.Application`)
- `IProtocolService` -> `IDailyRhythmMapService`
- DTOs:
  - `ProtocolDay` -> `DailyRhythmMapDay`
  - `ProtocolEvent` -> `DailyRhythmMapEvent`
  - `ProtocolEventType` -> `DailyRhythmMapEventType`
  - `ProtocolSummary` -> `DailyRhythmMapSummary`
- File: `DTOs/Protocol/ProtocolDtos.cs` -> `DTOs/DailyRhythmMap/DailyRhythmMapDtos.cs`

#### Infrastructure Layer (`src/GajaTrack.Infrastructure`)
- `ProtocolService` -> `DailyRhythmMapService`
- REST API endpoint: `/api/protocol` -> `/api/daily-rhythm-map`

#### Web UI Layer (`src/GajaTrack.Web`)
- Components:
  - `ProtocolPage.razor` -> `DailyRhythmMapPage.razor` (Route: `/daily-rhythm-map`)
  - `ProtocolChart.razor` -> `DailyRhythmMapChart.razor`
  - `ProtocolSummaryCell.razor` -> `DailyRhythmMapSummaryCell.razor`
- Nav Link: "24h Protocol" -> "Daily Rhythm Map"

#### Tests (`tests/GajaTrack.Tests`)
- Rename all classes and files matching `Protocol*Tests` to `DailyRhythmMap*Tests`.

## Verification Plan
1. **Compilation:** The solution must compile without errors.
2. **REST API:** Verify `/api/daily-rhythm-map` returns correct data.
3. **UI:** Verify navigation to `/daily-rhythm-map` works and the page renders correctly.
4. **Tests:** All existing tests (renamed) must pass.
