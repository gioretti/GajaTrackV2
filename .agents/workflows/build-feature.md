---
description: Core execution loop for building features natively with Antigravity
---

# Antigravity Workflow: Build Feature

This workflow defines the end-to-end process for implementing a new feature or fixing a bug in GajaTrack. As a native AI agent, you must execute these steps sequentially and meticulously.

1. **Elicitation (Planning Mode)**
   - Analyze the user's initial request.
   - **Crucial:** Activate the `product-requirements` skill. Do not accept vague requests. Ask "Why?" and define clear Acceptance Criteria (AC).
   - Ensure the request aligns with the overarching product vision.

2. **Blueprinting (Planning Mode)**
   - Activate the `architectural-design` skill. 
   - Draft an `artifact:implementation_plan` that strictly adheres to Clean Architecture and Domain-Driven Design (DDD).
   - The plan MUST include a detailed structural breakdown.
   - **User Gate:** Request formal USER approval on the implementation plan before proceeding to execution.

3. **Execution Setup (Execution Mode)**
   - Enter Execution Mode.
   - Break the approved plan down into a session `task` artifact checklist.
   - Create a dedicated Git branch using the `git-protocol` skill (e.g., `feat/track-name` or `fix/issue-name`).

4. **Atomic Implementation Loop (Execution Mode)**
   - For every single task in your `task` artifact:
     1. Activate the `test-driven-development` skill.
     2. Write the failing test FIRST. Watch it fail.
     3. Implement the minimal code required to make it pass.
     4. Refactor if necessary.
     5. Create an **Atomic Commit** following `git-protocol`.
     6. Mark the task as `[x]` in your session `task` artifact.
   - Repeat this sub-loop until all tasks are complete.

5. **Verification (Verification Mode)**
   - Enter Verification Mode.
   - If UI changes were made, activate the `ui-verification` skill (or use the browser tools directly) to validate the Blazor WebApp functionality.
   - Ensure all API endpoints behave as expected.

6. **Documentation Evaluation**
   - Ask yourself: "Did this feature change warrant an update to `docs/` or ADRs? Should we document new coding conventions?"
   - **Mandatory:** If the answer is yes, you MUST execute the file updates immediately in a new commit.

7. **Iterative Self-Review**
   - You must perform a harsh self-critique before presenting to the USER.
   - Activate the `architectural-review` skill.
   - Scrutinize your own diffs against the original requirements and the `architectural-design` rules.
   - Treat yourself as a strict Reviewer challenging the Developer. Iterate on fixes (Critique -> Fix -> Critique) until you propose 0 new changes.

8. **Commit, PR, & Cleanup**
   - Push your branch to origin.
   - Summarize the completion of the work to the USER using the `notify_user` tool, highlighting the tests written and the self-review performed.
   - Await USER instruction to merge.
   - Once approved, execute the Merge and Cleanup sequence defined in the `git-protocol` skill to finalize the feature.
