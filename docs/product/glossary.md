# Domain Glossary & Ubiquitous Language

## Purpose
GajaTrack strictly adheres to Domain-Driven Design (DDD). Maintaining a consistent vocabulary prevents architectural drift and UI confusion. AI Agents and developers **must** use these exact terms in code structure, variable naming, and UI text.

## Core Terminology

### BabyDay
The fundamental temporal unit in GajaTrack. It is a strictly enforced 24-hour logical window spanning from **06:00 to 06:00** local time, rather than a standard midnight-to-midnight calendar day. 

### Daily Rhythm Map
The automated 24-hour visualization of a `BabyDay`. It plots all recorded sessions and points on a contiguous timeline.

### Session
An activity that has both a `StartTime` and an `EndTime`. 
- **SleepSession:** A period where the baby is asleep.
- **CryingSession:** A period of sustained crying.
- **NursingFeed:** A breastfeeding session.

### Point
An activity mapped to a single moment in time (`Time` or `StartTime` where duration is negligible).
- **BottleFeed:** A feed using a bottle, characterized by its `Content` (Formula vs. Milk) and `AmountMl`.
- **DiaperChange:** Categorized by `Type` (Wet, Dirty, Mixed).

### Nap vs. Night Sleep
**Start-Time Attribution Rule:** A sleep session's entire duration is classified strictly by when it *starts*, regardless of when it ends.
- **Nap:** A `SleepSession` starting between **06:00 and 18:00** (Daytime). If the nap crosses over 18:00, the entire duration is still classified as a Nap.
- **Night Sleep:** A `SleepSession` starting between **18:00 and 06:00** (Nighttime). If the sleep crosses over 06:00, the entire duration is still classified as a Night Sleep and counts toward the `BabyDay` that started at 18:00 on the previous calendar day.
- **Night Waking:** An interruption between Night Sleep sessions.

## Restricted Terminology

To maintain domain purity, AI agents must **avoid** the following overly generic terms. If absolutely necessary, they may be used as a postfix/prefix (e.g., `DomainEvent`), but never as standalone core concepts:

- ❌ **Protocol** (Use: `Daily Rhythm Map`)
- ❌ **Event** (Use specific terms: `Session`, `Nap`, `Feed`, or the exact entity name)
- ❌ **Log** (Use: `Record` or the exact entity name)
- ❌ **Chart** (Use: `Map` when referring to the daily timeline, `Chart` is only acceptable for statistical aggregations like the sleep trends)
