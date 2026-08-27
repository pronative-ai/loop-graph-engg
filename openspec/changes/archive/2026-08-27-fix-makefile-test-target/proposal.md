## Why

Running `make test` failed with MSBuild error `MSB1001: Unknown switch --project` because `dotnet test` does not accept `--project` (which is specific to `dotnet run`), and was pointing at the console executable rather than the unit test suite. The project needs an accurate `test:` target in `Makefile` and a root `.sln` solution file so developer commands (`make test`, `dotnet test`) execute cleanly.

## What Changes

- Update `Makefile` target `test:` to execute `dotnet test tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj`.
- Create a root solution `LoopAgent.sln` incorporating `src/AksAgenticWorkflowConsole.csproj` and `tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj` to support direct root `dotnet build` and `dotnet test`.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agentic-workflow-core`: Update build and test automation requirements to ensure `make test` and root `dotnet test` reliably discover and execute the test project suite.

## Impact

- **`Makefile`**: Fixes the `test:` recipe.
- **`LoopAgent.sln`**: Root solution file referencing src and test projects.
