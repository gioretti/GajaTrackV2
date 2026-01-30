# ADR 001: SQLite for Persistence
status: Accepted
date: 2026-01-30

## Context
We need a database solution for "GajaTrackV2". The application is currently a single-tenant, self-hosted tool for personal use.

## Decision
We will use **SQLite** as the primary backing store.

## Rationale
-   **Simplicity**: Single file database, no server process to manage (Docker container is stateless except for the volume).
-   **Portability**: Easy to backup/restore (just copy the file).
-   **Performance**: More than sufficient for the expected volume (thousands of records, not millions/sec).

## Consequences
-   **Limitations**: Concurrency is limited compared to Postgres/SQL Server (single writer), but acceptable for a personal app.
-   **Migration**: If we scale to multi-tenant SAAS, migrating from SQLite to Postgres is supported by EF Core.
