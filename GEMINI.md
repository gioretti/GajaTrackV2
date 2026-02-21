# Project: Baby Gaja Tracking Application
A minimalist tracking application for baby behavior data. Focus: Speed and Data Integrity.

## ANTIGRAVITY WORKFLOW
This project uses Antigravity natively. The AI agent manages state, context, and execution modes (PLANNING, EXECUTION, VERIFICATION) within a unified session.

The core process is driven by the **`.agents/workflows/build-feature.md`** instruction set. You must always refer to and follow this workflow when building new features or fixing bugs.

## DEVELOPMENT PRINCIPLES
We strictly follow Andrej Karpathy's 4 principles for AI coding:
- **Think Before Coding:** Don't assume. Don't hide confusion. Surface tradeoffs. State your assumptions explicitly.
- **Simplicity First:** Minimum code that solves the problem. Nothing speculative. No abstractions for single-use code.
- **Surgical Changes:** Touch only what you must. Clean up only your own mess. Every changed line should trace directly to the user's request.
- **Goal-Driven Execution:** Define success criteria. Loop until verified. Transform tasks into verifiable goals (e.g. "Write tests, then make them pass").
## REPOSITORY MAP
- **Product Documentation:** `docs/product/` (Vision, Glossary, UI/UX Guidelines)
- **Features:** `docs/features/`
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
