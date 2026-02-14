# Specification: Git Workflow Evolution

## Goal
Transition from direct-to-master commits to a track-isolated branching strategy that preserves clean history and mandatory reviews.

## Requirements
- **Isolation:** Every track MUST have its own Git branch.
- **Naming:** Branch names must match the track ID (e.g., `013_GitWorkflowEvolution`).
- **Commits:** Maintain atomic commits within the branch.
- **Merge Strategy:** 
    1. Rebase track branch on top of `master`.
    2. Merge into `master` with `--no-ff` (No Fast Forward).
- **Review:** A code review is required before merging.

## Acceptance Criteria
- [ ] `GEMINI.md` updated with the new Git mandate.
- [ ] `conductor/workflow.md` updated with technical commands for rebase/merge.
- [ ] The "Surgical Changes" rule updated to include branch isolation.
