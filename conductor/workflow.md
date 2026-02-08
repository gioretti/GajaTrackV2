# Project Workflow: Baby Gaja Tracking Application

## Development Methodology
- **Track-Based State Management:** The `conductor/tracks.md` and individual track plans are the source of truth for project and feature state.
- **TDD (Test-Driven Development):** All features must begin with failing tests before implementation.
- **Clean Architecture & DDD:** Adhere to the boundaries defined in `docs/architecture.md`.
- **Atomic Commits:** Commit changes after every individual task.

## Persona-Based Orchestration (Conductor Flow)
1. **Product Owner:** Defines requirements via `spec.md` in the track folder.
2. **Software Architect:** Validates technical design via `plan.md` and manages ADRs.
3. **Software Developer:** Executes tasks from the approved `plan.md`.

## Definition of Done (DoD)
A track or feature is considered "Done" only when:
- **Product Requirement:** All Acceptance Criteria in `spec.md` are met and verified.
- **Technical Quality:** Code follows DDD boundaries and passes 80% test coverage.
- **Manual Verification:** UI-related features must be manually verified using **Chrome DevTools** (see below).
- **Architectural Review:** The Architect has issued a "Pass" on the final implementation summary.
- **Documentation:** Relevant ADRs and `docs/architecture.md` are updated.
- **Orchestration:** The track status in `conductor/tracks.md` is updated to `[x]`.

## Manual UI Verification Protocol
When implementing or modifying user interface elements, the Developer must:
1. **Launch Application:** Start the web application (e.g., `dotnet run`).
2. **Initialize Browser:** Use the `chrome-devtools` skill to navigate to the application URL.
3. **Execute Manual Test:** Perform the actions described in the Acceptance Criteria.
4. **Inspect State:** Use DevTools to verify network requests, console logs, or DOM state if necessary.
5. **Evidence:** Briefly document the verification result in the **Implementation Summary**.

## Requirements & Constraints
- **Test Coverage:** Minimum 80% code coverage required for all new logic.
- **Task Summaries:** Recorded directly in the Git commit messages.
- **File Writes:** All changes (ADRs, Docs, Code, Tracks) must be physical file writes to disk.

## Phase Completion & Checkpointing
At the end of each phase, the Developer must seek Architect approval via an "Implementation Summary" before proceeding.
- [ ] Task: Conductor - User Manual Verification '<Phase Name>' (Protocol in workflow.md)
