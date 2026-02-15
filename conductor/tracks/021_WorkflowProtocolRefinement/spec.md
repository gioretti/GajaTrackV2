# Specification: Workflow Protocol Refinement

## Problem Statement
Git instructions are currently spread across multiple files, leading to instruction fragmentation and potential errors by AI agents.

## Goals
- Centralize all technical Git SOPs into a dedicated `git-protocol` skill.
- Simplify `GEMINI.md` to be a pure "Constitutional" file that triggers specialized skills.
- Automate track cleanup via a `/complete-track` command.
