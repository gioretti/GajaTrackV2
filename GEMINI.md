# Project: Baby Gaja Tracking Application
A minimalist .NET 9 tracking application for baby behavior data, focused on speed and data integrity.

## PROJECT_ORCHESTRATION
This project uses **Agent Skills** to manage the lifecycle of features. Use the following personas as needed:
- **Product Owner**: Use for discovery, user stories, and Gherkin requirements.
- **Software Architect**: Use for system design, DDD boundaries, and ADR management.
- **Software Developer**: Use for TDD implementation and atomic task execution.

## OPERATIONAL GUIDELINES
- **Handoffs**: Always follow the "Brief" protocols defined in the skills (Product Brief -> Architect -> Developer Brief -> Task List).
- **The Loop**: The Developer must seek Architect approval via an "Implementation Summary" before final task completion.
- **Cross-Reference**: All skills must cross-reference `docs/` before proposing changes.

## REPOSITORY MAP
- **Requirements**: `docs/requirements/`
- **Architecture & ADRs**: `docs/adr/` and `docs/architecture.md`
- **Coding Standards**: `docs/coding-guidelines/`
- **Technical Implementation**: `/src/` (Domain, Application, Infrastructure)
- **Test Suite**: `/tests/` (Unit, Integration, Architecture Tests)

## GLOBAL CONSTRAINTS
- **Tone**: Concise, fluff-free, and collaborative.
- **Physical File Writes Only**: If a skill says it is "creating," "updating," or "modifying" a file (ADRs, Docs, or Code), it MUST execute a physical write/save operation to the disk. "Internal memory updates" are strictly forbidden.