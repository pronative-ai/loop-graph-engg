## Why

The codebase currently has minimal inline documentation, making it hard for new developers to self-review and understand the agentic workflow without deep traversal. The project will also be presented live (30-minute session) where key areas need to be called out to an audience, but there is no marker pointing the presenter to those areas. Comments improve developer comprehension, and a `HIGHLIGHT` marker gives the presenter a reliable anchor for the live walkthrough.

## What Changes

- Add concise, helpful comments to the C# source files explaining the purpose of core types, methods, and non-obvious control flow (Loop vs Graph paradigm, guardrails, terminal tool, human checkpoint store).
- Introduce a consistent `HIGHLIGHT` marker convention used as an additional tag/comment on the most important, presenter-ready code areas.
- Establish a light convention for when to comment, comment style, and when to use the `HIGHLIGHT` marker so the repo is shared and reviewed consistently.
- Keep behavior, public APIs, and build output unchanged (documentation-only, no behavioral change).

## Capabilities

### New Capabilities
- `code-comments-documentation`: A convention specifying when and how source files are commented for developer readability, and the `HIGHLIGHT` marker used to flag key areas for the live presentation.

### Modified Capabilities
<!-- No existing spec-level behavior changes; this introduces a new documentation convention. -->

## Impact

- **Code**: inline comments and `HIGHLIGHT` tags added across `src/` without altering logic, signatures, or build output.
- **Tests**: existing tests remain valid; no new behavior to cover, though the convention is documented.
- **Docs**: new convention guidance captured in the `code-comments-documentation` spec so current and future developers follow the same standard.
- **No breaking changes**: purely additive comments and documentation markers.
