# Project: Baby Gaja Tracking Application
A minimalist tracking application for baby behavior data. Focus: Speed and Data Integrity.

## PROJECT ORCHESTRATION (CONDUCTOR)
This project uses **Conductor** as the central engine. All work happens in **Tracks**.

### Personas & Roles
- **Conductor (Coordinator):** State management and track lifecycle.
- **Product Owner (PO):** Defines **WHAT** and **WHY** in `spec.md`.
- **Software Architect:** Technical design in `plan.md` and verification.
- **Software Developer:** Execution expert. **Forbidden from expanding scope or refactoring project structure without an approved plan update.**

## DEVELOPMENT PRINCIPLES
- **Surgical Changes:** Touch only what is explicitly in the `plan.md`. **Strictly clean up only your own mess.** Opportunistic refactoring is a PROTOCOL BREACH.
- **Git Discipline:** The Developer MUST activate and follow the **`git-protocol`** skill for all branch, commit, PR, and merge operations. No work outside a track branch.
- **Think Before Coding:** State assumptions explicitly. Surface tradeoffs. Push back if a simpler approach is possible.
- **Simplicity First:** Minimum code that solves the problem. Nothing speculative.
- **Goal-Driven Execution:** **NO PRODUCTION CODE WITHOUT A FAILING TEST FIRST.** Use the "Task -> Verify" loop.

## OPERATIONAL GUIDELINES
- **Track-Driven Development:** No work outside a Conductor track. Use `/conductor:implement`.
- **Mandatory Review Gate:** Merging into `master` is a privileged operation. Follow the review process defined in the `git-protocol` skill.
- **The Handoff:**
    1. **PO Discovery:** User Request -> `spec.md`.
    2. **Architect Design:** `spec.md` -> `plan.md`.
    3. **Developer Execution:** `plan.md` -> Implementation (TDD).
- **Verification:** Developer MUST seek Architect approval via **Implementation Summary** for every phase.
- **Cross-Reference:** Always verify against `docs/` and `conductor/` before proposing changes.

## REPOSITORY MAP
- **Conductor State:** `conductor/`
- **Requirements:** `docs/requirements/`
- **Architecture & ADRs:** `docs/adr/` and `docs/architecture.md`
- **Coding Standards:** `conductor/code_styleguides/`
- **Technical Implementation:** `/src/`
- **Presentation Layer:** `src/GajaTrack.Presentation/` (RestApi & WebApp)
- **Test Suite:** `/tests/`

## GLOBAL CONSTRAINTS
- **Runtime Environment:** win32 (Windows).
- **Shell Syntax:** Always use PowerShell-compatible syntax. Use `;` as a statement separator instead of `&&`.
- **Tone:** Concise, professional, zero fluff.
- **Physical File Writes Only:** All modifications must be saved to disk.
- **Atomic Tasks:** Small, verifiable units only.
- **Zero Orphaned Code:** Remove imports/variables/functions that YOUR changes made unused. Don't touch pre-existing dead code.
