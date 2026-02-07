# Specification: Orchestration Re-alignment

## Objective
Establish Conductor as the central orchestration engine for the Baby Gaja Tracking Application. This replaces manual, friction-heavy handoff protocols with a track-based automated workflow while retaining the expert personas of Product Owner, Architect, and Developer.

## Proposed Changes

### 1. GEMINI.md Overhaul
- Formalize Conductor as the primary "Coordinator".
- Redefine persona engagement via `/conductor:implement`.
- Map Conductor "Phases" to the existing Brief-based handoffs.

### 2. Skill Refinement
- **Product Owner:** Focus on requirement gathering and `spec.md` validation. Remove internal "Product Brief" formatting rules in favor of Conductor's `spec.md`.
- **Software Architect:** Focus on `plan.md` review and ADRs. Remove "Developer Brief" generation; instead, the Architect "Approves" or "Refines" the Conductor `plan.md`.
- **Software Developer:** Focus on atomic task execution. Use the Conductor `plan.md` as the source of truth for the task list.

### 3. Unified Workflow
- Use Conductor's `tracks.md` to maintain project state.
- Implement the "Definition of Done" within the Conductor `workflow.md`.

## Success Criteria
- A single `GEMINI.md` that clearly defines how Conductor interacts with Skills.
- Skills no longer contain redundant instructions for manual file generation (Briefs).
- A seamless transition from user request -> PO Discovery -> Architect Design -> Developer Task.
