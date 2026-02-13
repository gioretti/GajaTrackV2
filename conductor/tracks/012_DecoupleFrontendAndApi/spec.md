# Specification: Decouple Frontend and API

## Goal
Establish a clear separation between the Backend (REST API) and the Frontend (SPA).

## Requirements
- **GajaTrack.Web:** Must be a pure REST API. No Razor Components, no `App.razor`, no UI assets in source.
- **GajaTrack.Web.Client:** Must be a standalone Blazor WebAssembly application. It owns all CSS, JS, and the `index.html` bootstrapper.
- **Interactions:** The Frontend communicates with the Backend strictly via HTTP/JSON.

## Acceptance Criteria
1. `GajaTrack.Web` contains zero `.razor` files.
2. `GajaTrack.Web.Client` contains its own `wwwroot` with `index.html`.
3. Navigation and data fetching work correctly.
4. The solution follows the model: `Domain/Application <-> REST API <-> FrontEnd`.
