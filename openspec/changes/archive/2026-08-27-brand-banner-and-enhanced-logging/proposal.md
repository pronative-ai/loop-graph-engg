## Why

When the application starts, presenting a prominent, vibrant green brand banner (`pronative.ai`) with large font styling establishes brand identity and visual excellence. Enhancing log formatting, colors, contrast (e.g. using `ConsoleColor.Gray` for observations instead of low-contrast `DarkGray`), and structured visual badges ensures all terminal output during live walkthroughs is readable, professional, and easy for students and audiences to follow.

## What Changes

- Add a high-visibility, large ASCII brand banner in bright green (`ConsoleColor.Green`) upon startup in `ConsoleLogger.BrandBanner()`.
- Enhance terminal readability and visual contrast across all logs in `ConsoleLogger`:
  - Upgrade observations from `ConsoleColor.DarkGray` to `ConsoleColor.Gray` for clean readability on dark terminal themes.
  - Add structured formatting helpers for menus, badges, section dividers, and state transitions.
- Update `Program.cs` to render the prominent green brand banner and formatted startup logs upon initialization.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Update centralized console logging and startup display requirements to include the large green brand banner and enhanced readable color scheme.

## Impact

- **`src/Shared/ConsoleLogger.cs`**: Implements `BrandBanner()` in green, improves observation contrast and output aesthetics.
- **`src/Program.cs`**: Renders `ConsoleLogger.BrandBanner()` on application launch.
