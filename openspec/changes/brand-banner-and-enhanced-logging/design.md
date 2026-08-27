## Context

The terminal application serves as a demonstration tool for students and audiences learning AI agent workflows. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Provide a large-font, stylized brand banner for `pronative.ai` in bright `ConsoleColor.Green`.
- Enhance contrast and visual clarity across all log categories (improving observations to `ConsoleColor.Gray` from low-contrast `DarkGray`).
- Provide consistent menu and header formatting.

**Non-Goals:**
- Modifying underlying agent execution logic or model invocation.

## Decisions

### Decision 1: Render ANSI/ASCII large brand banner
- *Design*: Implement `ConsoleLogger.BrandBanner()` with an ASCII block banner and stylized subtitle rendered in `ConsoleColor.Green`.
- *Rationale*: Renders cleanly across Windows Terminal, PowerShell, and Unix shells without requiring external font assets.

### Decision 2: Enhance observation text color
- *Design*: Change `Observation` text from `ConsoleColor.DarkGray` to `ConsoleColor.Gray`.
- *Rationale*: `DarkGray` often suffers from poor contrast against standard black terminal backgrounds, whereas `Gray` offers crisp, clear readability while remaining visually distinct from `White` and `Cyan`.

## Risks / Trade-offs

- [Risk] Narrow terminal windows wrapping ASCII banner.
  - *Mitigation*: Ensure banner width is constrained to standard 80-column terminal dimensions.
