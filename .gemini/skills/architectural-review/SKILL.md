---
name: architectural-review
description: Strict code review engine for iterative self-critique. Use when: (1) In Step 6 of the build-feature workflow, (2) Before presenting completed code to the USER, (3) Validating if code violates DDD or Clean Architecture.
---

# Architectural Review (Self-Critique Skill)

This skill converts Antigravity into a harsh, unforgiving Code Reviewer. You must use this skill iteratively *against your own work* before you declare a task complete to the USER.

## The Iterative Critique Loop
When triggered, you must perform the following loop until you find ZERO new issues:

1. **Self-Critique:** Review all diffs generated in the current track against the checklist below.
2. **Fix:** If you find a violation, immediately fix it in a new atomic commit.
3. **Repeat:** Critique again. You cannot stop until the answer to all checks is "Pass."

## The Strict Checklist (Zero Tolerance)

### 1. Architectural Boundaries (DDD)
- [ ] **Dependency Direction:** Do any classes in the Domain layer reference Application, Infrastructure, or Presentation? *(If yes -> FAIL. Fix immediately.)*
- [ ] **Leakage:** Are EF Core attributes or HTTP concepts bleeding into the Application/Domain layer? *(If yes -> FAIL.)*

### 2. The KISS Mandate (Simplicity)
- [ ] **Speculative Code:** Did you add generic interfaces, base classes, or "helper" methods that are only used in one place "just in case we need them later"? *(If yes -> FAIL. Delete them.)*
- [ ] **Test-Only Logic:** Are there public properties or methods defined purely so tests can read them? *(If yes -> FAIL. Refactor tests to rely on behavior, not state.)*

### 3. Architecture Decision Records (ADR)
- [ ] **ADR Compliance:** Does the implementation violate any active decisions (e.g., UTC time handling) documented in `docs/adr/`? *(If yes -> FAIL.)*

### 4. Cleanliness & Scope
- [ ] **Orphaned Code:** Did your changes make external imports, variables, or private methods unused? *(If yes -> FAIL. Clean up your mess.)*
- [ ] **Scope Creep:** Did you refactor something unrelated to the feature requirements just because you saw it? *(If yes -> FAIL. Revert it. We only make surgical changes.)*
- [ ] **Naming:** Do names exactly match the ubiquitous language established in the requirements?

## Completion
Once you pass this checklist with no new issues, you may proceed to the `Commit & PR Handoff` step in the workflow and notify the USER.
