## Context

The project is a .NET 10 MAF console app (`src/`) with a Loop vs Graph paradigm demo, middleware guardrails, a terminal execution tool, and a human checkpoint store. It will be shared with developers for self-review and presented live (30 minutes), so code must read clearly on its own and expose presenter-ready anchors. See proposal.md - Why for motivation.

This is a documentation-only change: comments and `HIGHLIGHT` markers are added to existing source files without altering logic.

## Goals / Non-Goals

**Goals:**
- Add concise, intent-focused comments across `src/` covering core types, key methods, and non-obvious control flow.
- Introduce a `HIGHLIGHT` marker convention and apply it to the most presenter-relevant areas in a top-to-bottom presentable order.
- Keep the build, behavior, and public API surface identical.

**Non-Goals:**
- No logic, refactoring, namespace, or dependency changes.
- No new runtime features or new tests for behavior (existing tests must simply continue to pass).
- Not a rewrite of the README or external docs; only inline source comments are in scope.

## Decisions

- **Comment style**: Use single-line `//` comments for explanatory notes and keep XML doc summaries for public API surfaces (consistent with the project's existing convention). Explain *intent* and *why*, not a restatement of the code. → Alternative: heavy Javadoc-style block comments — rejected for verbosity and inconsistency with the lean C# style already used.
- **HIGHLIGHT marker format**: A single comment line of the form `// HIGHLIGHT: <short note>` placed immediately above the key statement. Distinctive and greppable so a reviewer can run `grep -n HIGHLIGHT src/*.cs` and obtain an ordered presentation walkthrough. → Alternative: a special region (`#region`) — rejected because regions can hide code and do not read as a self-documenting marker.
- **Placement strategy**: Comment each file's top-level responsibility first, then comment the Load-bearing control flow (graph wiring, guardrail interception, terminal execution, human checkpoint). `HIGHLIGHT` markers are reserved for the 5–7 most persuasive areas tied to the demo's narrative (Loop vs Graph, parallel agents, guardrails, deployment checkpoint).
- **No behavioral change**: Markers and comments are inert to the compiler; verified by keeping the build green and tests passing.

## Risks / Trade-offs

- [Comments drift out of date as code evolves] → Keep comments intent-focused and concise so they stay robust to minor implementation churn; PR reviewers treat stale comments as a review criterion.
- [Too many HIGHLIGHT markers dilute the presentation] → Limit markers to a small, curated set; the spec caps them to the key presentation areas.
- [No functional test proves quality of prose] → Rely on human review plus the discoverability scenario (search for `HIGHLIGHT`); prose quality is a review check, not a runtime assertion.
