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
- **Surgical Changes:** Touch only what is explicitly in the `plan.md`. **Strictly clean up only your own mess.** Opportunistic refactoring or "fixing" adjacent project structure is a PROTOCOL BREACH. If you see an improvement, you MUST stop and propose a new track or plan update.
- **Think Before Coding:** **Don't hide confusion.** State assumptions explicitly. Surface tradeoffs. If multiple interpretations exist, present them—don't pick silently. Push back if a simpler approach is possible.
- **Simplicity First:** Minimum code that solves the problem. Nothing speculative. **If you write 200 lines and it could be 50, rewrite it.** No abstractions for single-use code. No error handling for logically impossible scenarios.
- **Goal-Driven Execution:** Transform tasks into verifiable goals. **NO PRODUCTION CODE WITHOUT A FAILING TEST FIRST.** Use the "Task -> Verify" loop.

## OPERATIONAL GUIDELINES
- **Track-Driven Development:** No work outside a Conductor track. Use `/conductor:implement`.
- **Branch Isolation:** Every track MUST have its own Git branch named exactly after the Track ID (e.g., `013_GitWorkflowEvolution`). Direct commits to `master` are FORBIDDEN.
- **Merge Strategy:** Tracks are merged into `master` ONLY after a successful code review. Merging MUST use a `rebase` followed by a `merge --no-ff` strategy to preserve linear-ish history with merge bubbles.
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
- **Test Suite:** `/tests/`

## GLOBAL CONSTRAINTS
- **Tone:** Concise, professional, zero fluff.
- **Physical File Writes Only:** All modifications must be saved to disk.
- **Atomic Tasks:** Small, verifiable units only.
- **Zero Orphaned Code:** Remove imports/variables/functions that YOUR changes made unused. Don't touch pre-existing dead code.
