# Project Workflow: Baby Gaja Tracking Application

## Development Methodology
- **Track-Based State Management:** The `conductor/tracks.md` and individual track plans are the source of truth for project and feature state.
- **TDD (Test-Driven Development):** All features must begin with failing tests before implementation.
- **Clean Architecture & DDD:** Adhere to the boundaries defined in `docs/architecture.md`.
- **Atomic Commits:** Commit changes after every individual task.

## Persona-Based Orchestration
Following the protocols in `GEMINI.md`:
1. **Product Owner:** Defines requirements and Gherkin features (Product Brief).
2. **Software Architect:** System design, ADR management, and "Verdict" on implementation plans.
3. **Software Developer:** Task execution and "Implementation Summary" for Architect review.

## Requirements & Constraints
- **Test Coverage:** Minimum 80% code coverage required for all new logic.
- **Task Summaries:** Recorded directly in the Git commit messages.
- **File Writes:** All changes (ADRs, Docs, Code) must be physical file writes to disk.

## Phase Completion & Checkpointing
At the end of each phase, the Developer must seek Architect approval via an "Implementation Summary" before proceeding.
- [ ] Task: Conductor - User Manual Verification '<Phase Name>' (Protocol in workflow.md)