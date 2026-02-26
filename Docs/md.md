Got it. Below is a **Decision-style PBI** you can paste to the board that frames this as a **modernization/refactor** decision (not “rewrite from scratch”), hits the repo/VB/API points, and gives the brass a clear go/no-go with options.

---

## 🧾 Decision PBI

### Title

**EVWeb Modernization Decision: Repository, Language, and Architecture Refactor (VS2019 Retirement Readiness)**

### Type

Decision / Architecture / Technical Debt Reduction

### Problem Statement

EVWeb is currently a **.NET Framework 4.5 VB** application maintained using **Visual Studio 2019**. With VS2019 being retired, EVWeb requires a modernization path to remain supportable, secure, and maintainable. The current repository also contains **unrelated code**, increasing onboarding time, risk of accidental coupling, and build/deploy complexity.

### Decision Needed

Approve a modernization plan that:

1. migrates EVWeb into a **clean, dedicated repository**,
2. transitions codebase direction to **C#**,
3. selects an architecture strategy that enables a future **API-backed data layer** (without requiring immediate approval for a standalone API service).

### Goals / Desired Outcomes

* Ensure EVWeb remains supported after VS2019 retirement (tooling + maintainability)
* Reduce operational risk from legacy repo sprawl and mixed concerns
* Improve maintainability and staffing flexibility (C# standardization)
* Create a path toward cleaner separation of UI and data access (API-ready)

---

## Options Considered

### Option A — Minimal Lift (keep VB, keep current repo)

**Summary:** Keep current repo structure and VB, make only what’s necessary to keep running.

**Pros**

* Lowest short-term disruption
* Minimal initial engineering time

**Cons / Risks**

* Continues dependency on **VB expertise** (harder hiring/bench strength)
* Keeps repo sprawl and coupling risk
* Locks in legacy data access patterns
* Likely “pay later with interest” scenario

---

### Option B — Refactor & Modernize in Place (same repo, partial C#)

**Summary:** Incrementally modernize while keeping repo and structure.

**Pros**

* Smaller step than a full migration
* Can ship improvements gradually

**Cons / Risks**

* Repo remains noisy and risk-prone
* Mixed-language maintenance complexity (VB + C#)
* Hard to enforce boundaries / maintain architecture cleanliness

---

### Option C — Modernization Refactor (recommended)

**Summary:** Create a **new dedicated EVWeb repository**, re-platform the solution into a modern structure and transition to **C#** as the primary language. Preserve business behavior while improving structure and maintainability.

**Pros**

* Clean separation from unrelated code
* Supports modernization best practices (clean solution boundaries)
* C# standardization improves:

  * long-term maintainability
  * staffing flexibility (broader talent pool)
  * ecosystem/tooling/library compatibility
* Establishes an “API-ready” architecture path

**Cons / Risks**

* Requires upfront planning and parallel run strategy
* Requires stakeholder buy-in on approach and timeline

---

## Why C# (business-friendly reasoning)

* **Organization standardization**: C# is the dominant .NET language across enterprise stacks and internal tooling; easier to share patterns/libraries and cross-train staff.
* **Hiring & bench strength**: C# talent availability is significantly higher than VB in modern .NET shops.
* **Long-term maintainability**: modern .NET examples, libraries, and community support overwhelmingly target C# first.
* **Consistency**: reduced cognitive load and fewer one-off practices across codebases.

(We can keep this phrasing “best practices” oriented without dunking on VB too hard.)

---

## Architecture Path for Data Layer (no immediate separate API approval required)

### Approach 1 — Single ASP.NET MVC Project with Internal API Surface (recommended “step 1”)

* Build EVWeb as an MVC app where:

  * UI controllers serve pages/views
  * API controllers (or endpoints) serve JSON internally
  * Data access is behind a service layer (not scattered in UI)
* Benefits:

  * Sets clean separation patterns now
  * Enables later extraction into a standalone API with minimal churn
  * Keeps deployment simple initially (one app)

### Approach 2 — Separate API Project (future step, not required now)

* UI project calls API project over HTTP
* Benefits:

  * Strong boundary, independent scaling/security
* Tradeoff:

  * Needs more approvals, infra changes, service ownership

### Approach 3 — No API, but strict service/repository layer

* Keep DB calls in-process but behind a clean service layer (no direct ConnectionString usage in UI)
* Benefits:

  * Improves code health even if API extraction is delayed
* Tradeoff:

  * Doesn’t enforce boundaries as strongly as API separation

---

## Acceptance Criteria (for this Decision PBI)

* [ ] Stakeholders approve **Option A/B/C** with rationale
* [ ] Approval to create a **new dedicated EVWeb repository**
* [ ] Approval to make **C# the primary language** for the modernized solution
* [ ] Architecture approach selected for data access:

  * [ ] Single project MVC + internal API surface **OR**
  * [ ] Service layer only (API-ready) **OR**
  * [ ] Separate API project (if approved)
* [ ] High-level migration strategy agreed:

  * parallel run / cutover plan
  * definition of “same behavior” validation
* [ ] Risks acknowledged and mitigation documented

---

## Deliverables (from this PBI)

* Decision summary + chosen option
* Target repo structure proposal
* Language/stack decision documented
* Architecture decision documented (API path)
* Initial modernization backlog (next PBIs)

---

## Notes / Constraints

* Current state: EVWeb directly connects to DB via ConnectionString in UI layer
* API separation may require additional approvals; plan should support staged adoption
* Goal is to maintain business functionality while modernizing structure and maintainability

---

# Quick guidance on the “options conversation” (so you can steer it)

If you want the smoothest sell:

* Recommend **Option C** (new repo + C# + staged architecture)
* For API: recommend **Approach 1** (single MVC app with internal API endpoints + service layer)

  * It gives you a real architecture win without asking for “new infrastructure”
  * Later you can propose “extract API” as a follow-on once it proves value

---

If you want, I can also produce:

* a **one-paragraph executive summary** (the “email to brass” version),
* and the **next PBI** that follows this decision (e.g., “Create EVWeb Modernized Repo + Scaffold Solution + Baseline Authentication + Health Check”).
