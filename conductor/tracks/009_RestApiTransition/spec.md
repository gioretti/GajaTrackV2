# Specification: REST API Transition

## Goal
Transition the GajaTrack application to a REST API architecture. This decoupling allows the UI to run in Blazor WebAssembly (WASM) mode while the server handles data persistence and business logic. This is a foundational step for future mobile and offline support.

## Requirements

### 1. Protocol API
- **Endpoint:** `GET /api/protocol`
- **Parameters:** `startDate` (DateOnly), `endDate` (DateOnly).
- **Behavior:** Returns a list of `ProtocolDay` DTOs calculated by the server-side `ProtocolService`.
- **Constraint:** The server must handle all calculations; the API merely exposes the result.

### 2. Import API
- **Endpoint:** `POST /api/import/babyplus`
- **Parameters:** Multipart file upload (BabyPlus JSON).
- **Behavior:** Imports the data into the database.
- **Response:** Returns the count of records successfully imported/updated.

### 3. Architecture Changes
- UI components must be moved to the `GajaTrack.Web.Client` project.
- UI components must fetch data via the REST API when running in the browser.
- Dependency Injection (DI) should be configured to use direct service access on the server and HTTP client access on the client.

### 4. Implementation Strategy
- Step-by-step, atomic implementation.
- TDD: Every endpoint must have an integration test.
- No functional regressions.
- Skip "Most Recent First" and authentication for this phase to keep the scope focused.
