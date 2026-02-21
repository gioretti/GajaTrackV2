# Long-Term Product Vision: GajaTrack

## Purpose of this Document
This document defines the long-term vision, architectural boundaries, and future roadmap for GajaTrack. Its primary audience is **AI Software Agents** and developers maintaining the codebase. It provides the essential context needed to make independent, architecturally sound decisions that align with the project's ultimate goals.

## Core Identity
GajaTrack is a minimalist, high-performance tracking application for baby behavior data (sleep, feeding, diapers, crying, etc.). It prioritizes **speed of data entry** and **data integrity** for reliable medical review and pattern recognition. 

*If a proposed change introduces UI fluff, slows down data entry, or compromises the integrity of historical data, it violates the core identity.*

## Roadmap & Future Features
Agents must design current implementations with these future requirements in mind:

1. **Extensibility of Metrics:** The system must support adding new tracking entities (e.g., eating solids, temperature, medications) without requiring massive refactoring of the core engine.
2. **Multi-Baby Support:** The domain model must eventually support tracking multiple profiles concurrently.
3. **Data Mutation:** Users will need full CRUD capabilities (Update/Delete) for inserted records, requiring a robust way to navigate and query historical data.
4. **Hardware Integrations:** The platform will support mobile apps that allow users to map tracking events to physical actions, such as scanning NFC tags.

## Technical & Architectural Vision

### 1. The Multi-Device & Sync Strategy
The final technology stack for hosting and deployment is still under evaluation (e.g., self-hosted vs. SaaS). However, the architecture must support the following:
- **Offline-First / Immediate Sync:** Mobile/PWA clients must be able to operate offline, capture data, and synchronize immediately with a central data store (similar to Firebase paradigms) when online.
- **Desktop Analytics:** Regardless of the mobile capture strategy, a full desktop web application must remain available to leverage larger screens for complex data visualization (like the Daily Rhythm Map) and analysis.

### 2. The Absolute "Hard Boundaries"
When working on GajaTrack, AI agents **MUST NOT** cross these boundaries under any circumstances:

*   **The Domain is Sacred:** Never compromise the Domain layer for a "quick UI fix". The `GajaTrack.Domain` layer must remain pure, containing only business logic and zero infrastructure or presentation dependencies.
*   **The 06:00 to 06:00 Boundary:** The 24-hour logical day boundary (from 06:00 to 06:00 local time) is the foundational rule of the application's temporal logic. This boundary must be respected across the entire stack—from domain intersection calculations to UI rendering.
*   **Test Driven Integrity:** Tests are sacred. No production logic should be shipped without a corresponding, passing test that proves its correctness. Breaking tests to force a feature through is strictly forbidden.
*   **Time is UTC:** All time-based data must be converted to, stored as, and manipulated as UTC at the application boundaries. Local time is a presentation concern only.

## Agent Guidelines
When proposing solutions:
1. **Challenge Inconsistencies:** If a user request contradicts the boundaries listed above, you must push back, state the tradeoff, and suggest an architecture-compliant alternative.
2. **Surgical Precision:** Touch only what is necessary to fulfill the spec. Opportunistic refactoring outside the current track's scope is a protocol breach. 
3. **Think Long-Term:** Ensure that today's database schema or API contract changes won't artificially block the roadmap (e.g., hardcoding a single baby context when establishing a new data flow).
