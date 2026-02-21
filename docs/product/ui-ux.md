# UI & User Experience Guidelines

## Purpose
This document outlines the interaction paradigms and aesthetic constraints for GajaTrack. AI Agents generating frontend code must adhere to these rules to ensure the application remains fast, accessible, and fit for its specific use case.

## The Primary Use Case Matrix
Every UI decision must be evaluated against the "3 AM Constraint":
> **The user is a parent holding a baby at 3:00 AM, using a smartphone with one hand, operating highly fatigued.**

## Interaction Guidelines

### 1. Speed-First Data Entry
- **Zero-Friction:** Minimize taps and clicks. Do not hide primary tracking actions behind dropdowns or hamburger menus.
- **Sensible Defaults:** Forms should auto-populate with the most likely scenario (e.g., defaulting to the current time, defaulting to the last used bottle amount).
- **One-Handed Operation:** Critical action buttons should be large enough and positioned appropriately for easy thumb reach on mobile viewports.

### 2. Information Architecture
- **Information Density over Whitespace:** Parents need to see pattern data at a glance. Do not pad charts or tables with excessive whitespace. The Daily Rhythm Map must be compact and easily scannable.
- **Visual Contrast:** Use distinct, immutable color-coding and shapes for different entity types (e.g., Sleep is a solid bar, Nursing is a triangle, Crying is a wavy line) so data can be interpreted without reading text labels.

## Aesthetic Constraints

### 1. Minimalism & Performance
- **No Fluff:** Remove decorative elements, complex drop-shadows, or unnecessary borders that do not convey data meaning.
- **Zero Animation Overhead:** Do not implement UI animations (fades, slides, bounces) that artificially delay the user from logging data or viewing the map. Interaction must feel instantaneous.
- **Framework Agnostic:** While currently utilizing Bootstrap, the core styling must remain as "vanilla" and simple as possible to ensure fast load times and easy migration if the technological stack evolves.

### 2. The Native Feel
- **System Fonts:** Use native system font stacks for zero-latency text rendering.
- **Predictable Patterns:** Use standard paradigms (e.g., standard HTML `<input type="date">`) rather than complex custom date-pickers, as native inputs often provide the best accessibility and mobile keyboard integration.
