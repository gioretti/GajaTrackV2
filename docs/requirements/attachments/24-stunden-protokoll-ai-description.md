# 24-Stunden-Protokoll — Detailed Specification and Description

## Purpose of the Document

This document describes a **24-hour behavioral tracking protocol** (“24-Stunden-Protokoll”) used in pediatric sleep medicine to record and analyze a child’s sleep–wake behavior.

The goal is to visually document, across a continuous 24-hour timeline, the following behaviors:
- Sleep phases
- Wake phases
- Crying / fussing
- Meals or drinking
- Bedtime (moment the child is put into bed)

The output is a **time-based horizontal protocol graph**, repeated for multiple consecutive days.  
This description is intended to allow an AI or software system to **recreate the protocol graph exactly**, without access to the original PDF.

---

## Institutional Context

The protocol originates from:

- Universitäts-Kinderspital Zürich  
- Department: Entwicklungspädiatrie (Developmental Pediatrics)  
- Zentrum für Schlafmedizin (Center for Sleep Medicine)

The protocol is designed to be **filled manually by parents or caregivers**, then interpreted by medical professionals.

---

## Document Title

**24-Stunden-Protokoll**  
(24-Hour Protocol / 24-Hour Log)

---

## Metadata Fields (Header Section)

At the top of the document, the following fields are present and intended to be filled manually:

- Name (child’s name)
- Geburtsdatum (date of birth)
- Alter (age)

These fields identify the subject of the protocol.

---

## Overall Layout

The main content is a **large grid/table** with:

- **Rows representing calendar days**
- **Columns representing time of day**
- Each row visualizes **one full 24-hour period**

The structure resembles a **Gantt-style timeline**, with multiple event types overlaid on the same time axis.

---

## Time Axis (Columns)

### Time Range

The horizontal time axis spans **24 continuous hours**, starting and ending at **06:00**.

Exact sequence:

06:00 → 07 → 08 → 09 → 10 → 11 → 12 → 13 → 14 → 15 →  
16 → 17 → 18 → 19 → 20 → 21 → 22 → 23 → 24 →  
01 → 02 → 03 → 04 → 05 → 06

Notes:
- “24” is used instead of “00”
- The timeline continues seamlessly after 24 with 01–06
- This creates a **linear representation of a circular 24-hour cycle**
- The start of day is **06:00**, not midnight

### Resolution

- Columns are labeled hourly
- Entries may span partial hours
- Sub-hour precision is allowed and expected

---

## Date Axis (Rows)

Each row corresponds to **one calendar day**, labeled on the left using day and month format (e.g. `25.3.`).

Multiple consecutive dates are shown vertically.  
The protocol is intended to be filled for **14 consecutive days**, even if fewer rows are visible per page.

---

## Behavior Categories and Visual Encoding

The protocol uses **symbols instead of colors**. All markings are drawn manually.

### 1. Sleep Phases
- Represented by a **horizontal line**
- The line spans the full duration of sleep
- Continuous sleep is shown as one continuous line

### 2. Wake Phases
- Represented by **empty / unmarked space**
- Wakefulness is implicit (absence of symbols)

### 3. Crying / Fussing
- Represented by **wavy lines**
- Indicates periods of crying or quengeln (fussing)
- Can overlap with wake time or transitions

### 4. Meals / Drinking
- Represented by **triangles**
- Each triangle marks a feeding or drinking event
- These are point-in-time events, not durations

### 5. Bedtime (Putting Child into Bed)
- Represented by a **downward arrow**
- Marks the moment the child is put into bed
- This is an event marker, not sleep itself

---

## Legend and Instructions

The protocol includes a textual legend explaining the symbols and how to mark them.

Instructions specify:
- The protocol should be filled continuously over **14 days**
- Behaviors should be marked directly within the corresponding time columns
- Accuracy is important to allow medical interpretation of sleep–wake patterns

---

## Visual Style

- Black-and-white only
- Thin grid lines
- No background shading or colors
- Designed for handwritten input
- Clinical / medical form aesthetic

---

## Conceptual Model of the Graph

From a software perspective, the protocol represents:

- A **time-series visualization**
- X-axis: continuous 24-hour timeline (06:00 → 06:00)
- Y-axis: consecutive calendar days
- Multiple event types overlaid per day

### Event Types by Nature

- **Duration-based events**:
  - Sleep
  - Crying / fussing

- **Instant events**:
  - Meals / drinking
  - Bedtime

Wake time is inferred as the absence of sleep markings.

---

## Key Requirements for Software Replication

A correct implementation must support:

- A fixed 24-hour horizontal axis starting at 06:00
- One row per calendar day
- Continuous spans across hour boundaries
- Overlaying multiple event types
- Distinct visual representations:
  - Horizontal bars (sleep)
  - Wavy lines (crying)
  - Triangles (feeding)
  - Arrows (bedtime)
- Empty space representing wakefulness

The resulting visualization should resemble a **medical sleep protocol chart**, not a generic calendar or chart.

---

## Summary

This protocol is a structured, visual sleep–wake diary for children, designed for clinical analysis.  
It combines time-based duration tracking with discrete event markers, across multiple days, using a standardized symbolic language.

Any software or AI recreating this protocol must preserve:
- The 06:00–06:00 timeline
- The symbol semantics
- The multi-day grid structure
- The clinical clarity of the visualization
