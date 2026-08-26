# Proposal: Rename Namespace and Consolidate Using Statements

## Summary

Remove the `Aks` prefix from the namespace (`AksAgenticWorkflowConsole` → `AgenticWorkflowConsole`) and consolidate all `using` statements into a single `GlobalUsings.cs` file.

## Motivation

- **Namespace clarity**: The `Aks` prefix implies AKS-specific code, but this is a general agentic workflow console. The shorter `AgenticWorkflowConsole` is more accurate.
- **Using hygiene**: Scattered `using` statements across every file add visual noise. A `GlobalUsings.cs` file centralizes imports and follows modern .NET conventions.

## Scope

- All `.cs` files: Rename namespace from `AksAgenticWorkflowConsole` to `AgenticWorkflowConsole`
- New file: `src/GlobalUsings.cs` with all unique `using` statements
- All `.cs` files: Remove individual `using` statements (moved to global)

## Out of Scope

- Functional behavior changes
- New features
- Project file changes (namespace is in code, not csproj)

## Acceptance Criteria

- All classes use namespace `AgenticWorkflowConsole`
- `src/GlobalUsings.cs` exists with all required `using` statements
- No duplicate `using` statements in individual files
- Build succeeds with `dotnet build`