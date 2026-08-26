## Why

The build process generates warnings that should be resolved to maintain code quality and security. The OpenTelemetry.Api package has a known moderate severity vulnerability (GHSA-g94r-2vxg-569j) that needs to be addressed by updating to a secure version.

## What Changes

- Update OpenTelemetry.Api package to a version without known vulnerabilities
- Ensure all NuGet packages are at their latest stable versions
- Verify build completes without warnings

## Capabilities

### New Capabilities

None - this is a maintenance fix.

### Modified Capabilities

None - no spec-level behavior changes.

## Impact

- NuGet package version updates in .csproj file
- No code changes required
- No behavioral changes to the application
