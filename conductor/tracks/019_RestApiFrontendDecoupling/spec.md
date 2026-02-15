# Specification: REST API Frontend Decoupling

## Problem Statement
The current Blazor frontend (Web.Client) is directly injecting backend services (e.g., `IDailyRhythmMapService`). This violates the Clean Architecture principle of decoupling the Presentation layer from the Application/Domain layer via a stable contract (the REST API). It also prevents the frontend from being hosted independently or scaling separately from the backend.

## Goals
- Ensure all data fetching and operations in the frontend are performed via `HttpClient` calls to the REST API.
- Remove direct dependencies on Application services from the frontend components.
- Standardize the communication contract between the Blazor WASM client and the ASP.NET Core backend.

## User Stories
As a **Developer**, I want the frontend to use the REST API for all data operations, so that the architecture remains clean and the frontend is truly decoupled from the backend implementation.

## Acceptance Criteria

### 1. Daily Rhythm Map Page
- **Given** I am on the Daily Rhythm Map page
- **When** the page loads
- **Then** the data must be fetched from `/api/daily-rhythm-map` using `HttpClient`
- **And** the `IDailyRhythmMapService` must NOT be injected into the component.

### 2. Home Page (Stats)
- **Given** I am on the Home page
- **When** the page loads
- **Then** the stats must be fetched from `/api/stats` using `HttpClient`
- **And** any backend service injection must be removed.

### 3. Baby+ Import Page
- **Given** I am on the Import page
- **When** I upload a file
- **Then** the upload must be sent to `/api/import/babyplus` using `HttpClient`
- **And** the `IBabyPlusImportService` must NOT be injected into the component.

### 4. Dependency Injection Cleanup
- **Given** the frontend application
- **When** it starts
- **Then** only the necessary DTOs and API clients should be registered in the Client container
- **And** backend persistence/logic services should not be registered in the Client project.
