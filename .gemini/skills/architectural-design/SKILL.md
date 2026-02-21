---
name: architectural-design
description: Technical design expert for Clean Architecture and DDD. Use when: (1) Drafting an implementation_plan.md, (2) Designing system architecture or API contracts, (3) Making structural changes to the codebase.
---

# Architectural Design (Planning Skill)

Before execution begins, you must explicitly plan the technical approach. This skill enforces strict adherence to Domain-Driven Design (DDD) and Clean Architecture in GajaTrack.

## Core Principles
- **Clean Architecture & DDD:** Enforce strict boundaries between Domain, Application, and Infrastructure (as defined in `docs/architecture.md`).
- **Dependency Rule:** Dependencies must ALWAYS point inward toward the Domain layer. The Domain layer has zero outgoing dependencies.
- **Maintain History:** Adhere to and maintain Architectural Decision Records (ADRs) in `docs/adr/`.
- **KISS Mandate:** Obsessively keep implementation simple. Reject proposals for over-engineering or speculative generalization.

## Operational Workflow (The Blueprinting Phase)
1. **Review Specification:** Read the requirements established during the Elicitation phase in the current session.
2. **Draft the Plan:** When generating the `artifact:implementation_plan`, structure the proposed changes layer by layer:
   - **Domain:** Which entities/value objects change? (No external dependencies!)
   - **Application:** Which DTOs/Services change?
   - **Infrastructure:** Which EF Core configs or external integrations change?
   - **Presentation:** Which API controllers or Blazor components change?
3. **ADR Check:** If this introduces a new pattern or massive structural change, propose a new ADR in the plan.
4. **Validation:** Review your own plan. Ensure names are precise contextually and follow the C# guides.

## Constraints
- Do NOT begin the execution loop until the USER explicitly approves the implementation plan artifact you generated using this skill.
