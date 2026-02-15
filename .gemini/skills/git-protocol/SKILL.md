# Skill: Git Protocol

You are an expert in Git workflow and project hygiene. You MUST be activated whenever Git operations (branching, committing, merging) are required.

## 1. Pre-Implementation (Phase 0)
- **Commit Boundary Identification:** Before writing any implementation code, you MUST identify logical commit boundaries in the `plan.md`.
- **User/Architect Approval:** The proposed commit split MUST be shared with the User/Architect for approval before the first commit.

## 2. Branch Management
- **Isolation:** Every track must have its own branch named exactly after the Track ID (e.g., `018_BabyDayDomainModel`).
- **Forbidden:** No direct commits to `master`.

## 3. Commit Execution
- **Frequency:** Commit as soon as a boundary identified in Phase 0 is reached. 
- **Messages:** Use conventional commits (`feat:`, `refactor:`, `test:`). Use `-m` for multi-line details.

## 4. Pull Request & Review Gate
- **Submission:** Upon completing implementation, create a GitHub Pull Request (PR) from the track branch to `master`.
- **Mandatory Review:** All PRs must be reviewed by the **Software Architect** skill and approved by the **User**.
- **Addressing Feedback:** Address all review comments with new atomic commits. Mark comments as resolved only after the fix is pushed.
- **Merge Block:** **The Developer MUST NOT merge the PR if there are any unresolved comments, open discussions, or failing tests.**

## 5. The Integration Sequence (Merge)
When the PR is approved by the User:
1. **Sync:** `git checkout <TRACK_BRANCH>; git fetch origin; git rebase origin/master`.
2. **Push:** `git push origin <TRACK_BRANCH> --force-with-lease`.
3. **API Merge:** Execute a non-fast-forward merge via the GitHub API (`merge_method: "merge"`) to preserve merge bubbles on a linear history.

## 6. The Track Completion Routine (Cleanup)
Immediately after a successful merge (manually or via `/complete-track`):
1. **Sync Master:** `git checkout master; git pull origin master`.
2. **Local Cleanup:** `git branch -d <TRACK_BRANCH>`.
3. **Registry Update:** Mark track `[x]` in `conductor/tracks.md`.
4. **Index Update:** Mark status `COMPLETED` in `conductor/tracks/<ID>/index.md`.
5. **Remote Cleanup:** Ensure the remote branch is deleted (usually handled by the API merge).
