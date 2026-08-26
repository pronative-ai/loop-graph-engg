## Context

The build process generates warnings about a vulnerable OpenTelemetry.Api package (NU1902). This is a transitive dependency from Microsoft.Agents.AI packages. The vulnerability (GHSA-g94r-2vxg-569j) is moderate severity and should be addressed.

## Goals / Non-Goals

**Goals:**

- Remove all build warnings
- Update vulnerable packages to secure versions
- Maintain compatibility with existing code

**Non-Goals:**

- Refactoring application code
- Adding new features
- Changing package versions unnecessarily

## Decisions

### Decision: Update OpenTelemetry.Api

**Choice**: Update OpenTelemetry.Api to the latest stable version that resolves the vulnerability.

**Rationale**: The vulnerability is in a transitive dependency. Updating to a patched version eliminates the security risk without code changes.

**Alternatives considered**:

- Suppress warnings: Not recommended as it leaves the vulnerability unaddressed
- Remove OpenTelemetry: Not feasible as it's a required dependency

### Decision: Use PackageReference for Direct Control

**Choice**: Add explicit PackageReference for OpenTelemetry.Api to control its version.

**Rationale**: Transitive dependencies can't be directly controlled. Adding an explicit reference ensures the patched version is used.

**Alternatives considered**:

- Wait for upstream update: May take time, leaves vulnerability exposed
- Fork packages: Overkill for a simple version update

## Risks / Trade-offs

[Version compatibility] → The updated package may have API changes. Mitigation: Check release notes and test build.

[Transitive dependency conflicts] → Other packages may require specific versions. Mitigation: Use NuGet restore to verify resolution.
