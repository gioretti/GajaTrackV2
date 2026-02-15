# Specification: Project Renaming & Realignment

## Problem Statement
The current names `GajaTrack.Web` and `GajaTrack.Web.Client` are generic and nested within a folder that also contains ".Web". This creates naming redundancy and doesn't clearly reflect the roles of the projects in our decoupled architecture.

## Goals
- Rename projects to `GajaTrack.RestApi` and `GajaTrack.WebApp`.
- Realign the physical folder structure to use a clean `Presentation` parent folder.
- Update all namespaces, references, and the solution file.

## Acceptance Criteria
1. Backend project renamed to `GajaTrack.RestApi`.
2. Frontend project renamed to `GajaTrack.WebApp`.
3. Parent folder renamed to `src/GajaTrack.Presentation`.
4. Solution (`.sln`) updated and building successfully.
5. All namespaces in `.cs` and `.razor` files updated to match new names.
6. Local verification: App runs and UI communicates with API.
