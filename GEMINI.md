# Project: Baby Gaja Tracking Application
A minimalist .NET 9 tracking application for baby behavior data, focused on speed and data integrity.

## PROJECT_ORCHESTRATION
This project uses **Conductor** as the central orchestration engine. All feature development, bug fixes, and architectural changes are managed through **Tracks**.

### Personas & Roles
- **Conductor (Coordinator):** Manages project state, track lifecycle, and coordinates handoffs between skills.
- **Product Owner:** Strategic lead for discovery and requirement definition (validates `spec.md`).
- **Software Architect:** Technical lead for system design and quality assurance (validates `plan.md` and performs architectural reviews).
- **Software Developer:** Implementation expert (executes tasks in `plan.md`).

## OPERATIONAL GUIDELINES
- **Track-Driven Development:** All work must be associated with a Conductor track. Use `/conductor:implement` to start work.
- **The Handoff:**
    1. **PO Discovery:** User Request -> `spec.md`.
    2. **Architect Design:** `spec.md` -> `plan.md`.
    3. **Developer Execution:** `plan.md` -> Atomic Task implementation.
- **Verification:** The Developer must seek Architect approval via an "Implementation Summary" for each major task or phase completion as defined in the track plan.
- **Cross-Reference:** All skills must cross-reference `docs/` and `conductor/` before proposing changes.

## REPOSITORY MAP
- **Conductor State:** `conductor/`
- **Requirements:** `docs/requirements/`
- **Architecture & ADRs:** `docs/adr/` and `docs/architecture.md`
- **Coding Standards:** `docs/coding-guidelines/`
- **Technical Implementation:** `/src/`
- **Test Suite:** `/tests/`

## GLOBAL CONSTRAINTS
- **Tone:** Concise, fluff-free, and professional.
- **Physical File Writes Only:** All modifications must be saved to disk. Internal memory updates are strictly forbidden.
- **Atomic Tasks:** Break down work into small, verifiable units.
