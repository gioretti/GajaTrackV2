# Skill: Git Protocol

You are an expert in Git workflow and project hygiene. You MUST be activated whenever Git operations (branching, committing, merging) are required.

## 1. Pre-Implementation (Phase 0)
- **Commit Boundary Identification:** Before writing any implementation code, you MUST identify logical commit boundaries in the implementation plan artifact.
- **User Approval:** The proposed commit split MUST be shared with the USER for approval before the first commit.

## 2. Branch Management
- **Isolation:** Every track must have its own branch named exactly after the Track ID (e.g., `018_BabyDayDomainModel`).
- **Forbidden:** No direct commits to `master`.

## 3. Commit Execution
- **Frequency:** Commit as soon as a boundary identified in Phase 0 is reached. 
- **Messages:** Use conventional commits (`feat:`, `refactor:`, `test:`). Use `-m` for multi-line details.

## 4. Pull Request & Review Gate
- **Submission:** Upon completing implementation, create a GitHub Pull Request (PR) from the feature branch to `master`.
- **Mandatory Review:** All work must pass the internal `architectural-review` self-critique loop BEFORE PR creation, and the PR must be approved by the **USER**.
- **Addressing Feedback:** Address all review comments with new atomic commits. Mark comments as resolved only after the fix is pushed.
- **Merge Block:** **You MUST NOT merge the PR if there are any unresolved comments, open discussions, or failing tests.**

## 5. The Integration Sequence (Merge)
When the PR is approved by the User:
1. **Sync:** `git checkout <TRACK_BRANCH>; git fetch origin; git rebase origin/master`.
2. **Push:** `git push origin <TRACK_BRANCH> --force-with-lease`.
3. **API Merge:** Execute a non-fast-forward merge via the GitHub API (`merge_method: "merge"`) to preserve merge bubbles on a linear history.

## 6. The Feature Completion Routine (Cleanup)
Immediately after a successful merge:
1. **Sync Master:** `git checkout master; git pull origin master`.
2. **Local Cleanup:** `git branch -d <FEATURE_BRANCH>`.
3. **Remote Cleanup:** Ensure the remote branch is deleted.
