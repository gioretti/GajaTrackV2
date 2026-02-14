# Project Workflow: Baby Gaja Tracking Application

## Development Methodology
- **Track-Based State Management:** The `conductor/tracks.md` and individual track plans are the source of truth for project and feature state.
- **TDD (Test-Driven Development):** All features must begin with failing tests before implementation. Developers MUST activate and follow the `test-driven-development` skill for all behavioral changes.
- **Clean Architecture & DDD:** Adhere to the boundaries defined in `docs/architecture.md`.
- **Atomic Commits:** Commit changes after every individual task.

## Persona-Based Orchestration (Conductor Flow)
1. **Product Owner:** Defines requirements via `spec.md` in the track folder.
2. **Software Architect:** Validates technical design via `plan.md` and manages ADRs.
3. **Software Developer:** Executes tasks from the approved `plan.md`.

## Git Branching & Merging Protocol
Every track is isolated in its own branch to ensure clean integration and review cycles.

### Phase A: Isolation & Development
1.  **Branch Creation:** `git checkout -b <Track_ID>` (e.g., `git checkout -b 014_SeparateTestProjects`).
2.  **Development:** Execute atomic commits on the track branch.
3.  **Validation:** Ensure all tests pass and verification is complete.

### Phase B: MANDATORY REVIEW GATE (GitHub PR)
1.  **Create Pull Request:** The Developer MUST create a Pull Request on GitHub from the `<Track_ID>` branch to `master`.
2.  **Review Loop:**
    - The Developer MUST monitor the PR for feedback.
    - Use `pull_request_read` (method: `get_review_comments`) to ingest user feedback directly from GitHub.
    - Address comments locally, commit, and push updates to the track branch.
3.  **Approval:** Wait for the user to approve the PR on GitHub or provide explicit confirmation in chat.

### Phase C: Integration (via GitHub API)
Only execute after Phase B approval:
1.  **Rebase:** `git checkout <Track_ID>`, then `git rebase master`. This aligns the track branch with the current master to ensure a clean merge.
2.  **Sync:** `git push origin <Track_ID> --force-with-lease` to update the PR with the rebased commits.
3.  **Merge:** Use the GitHub API (`merge_pull_request`) with `merge_method: "merge"`. 
    - **Why:** This ensures a merge commit is created (the "merge bubble") on top of your rebased commits, maintaining the clear visual history requested.
    - **Context:** Avoid `"squash"` (which loses commit history) or `"rebase"` (which skips the merge bubble). Using `"merge"` after a local rebase is the only way to achieve the specific history structure defined in the project.
4.  **Cleanup:**
    - `git checkout master`
    - `git pull origin master`
    - **Crucial:** `git branch -d <Track_ID>` (The Developer MUST physically delete the local branch immediately after pulling to keep the repository hygienic).
    - `git remote prune origin` (Removes stale tracking references).

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
1. **Launch Application:** Start the web application using a **detached process** (e.g., a Windows Scheduled Task or an independent background job). This prevents CLI shell transitions from breaking the Blazor circuit (WebSocket connection).
   - *Protocol:* Use `schtasks` to create a one-time task that runs the application on a dedicated port.
2. **Initialize Browser:** Use the `chrome-devtools` skill to navigate to the application URL.
3. **Execute Manual Test:** Perform the actions described in the Acceptance Criteria.
4. **Inspect State:** Use DevTools to verify network requests, console logs, or DOM state if necessary.
5. **Evidence:** Briefly document the verification result in the **Implementation Summary**.
6. **Cleanup:** Physically terminate the detached process and delete any temporary tasks (e.g., `schtasks /delete`) once verification is complete.

## Requirements & Constraints
- **Test Coverage:** Minimum 80% code coverage required for all new logic.
- **Git Protocol:** Commit messages MUST include a detailed summary of the "what" and "why" in the message body, matching the style of the final session summary.
- **File Writes:** All changes (ADRs, Docs, Code, Tracks) must be physical file writes to disk.

## Phase Completion & Checkpointing
At the end of each phase, the Developer must seek Architect approval via an "Implementation Summary" before proceeding.
- [ ] Task: Conductor - User Manual Verification '<Phase Name>' (Protocol in workflow.md)
