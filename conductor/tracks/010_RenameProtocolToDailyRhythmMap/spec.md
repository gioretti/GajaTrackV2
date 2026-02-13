# Specification: Rename Protocol to Daily Rhythm Map

## Problem
The term "Protocol" is clinical and rigid. The user wants a more descriptive and human name: "Daily Rhythm Map".

## Goals
- Rename all "Protocol" occurrences to "DailyRhythmMap" (code) and "Daily Rhythm Map" (UI).
- Maintain functionality while improving ubiquitous language.

## User Stories
As a parent, I want to see a "Daily Rhythm Map" so that I can understand my baby's daily flow in a way that feels human and intuitive.

## Acceptance Criteria
- [ ] No mention of "Protocol" remains in the UI.
- [ ] No mention of "Protocol" remains in the Domain/Application/Infrastructure layers (except possibly external library references if any).
- [ ] API endpoint is updated to /api/daily-rhythm-map.
- [ ] Application continues to function identically.
- [ ] All tests pass after being updated to the new naming.
