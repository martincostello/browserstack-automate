# Coding Agent Instructions

This file provides guidance to coding agents when working with code in this repository.

## Build, test, and lint

- Preferred full validation is `./build.ps1` from the repository root. It bootstraps the exact .NET SDK from `global.json`, packs `src\MartinCostello.BrowserStack.Automate`, then runs tests unless `-SkipTests` is passed. Per `.github\CONTRIBUTING.md`, local changes should build cleanly with no compiler warnings and passing tests.
- To skip tests while validating packaging/build logic, run `./build.ps1 -SkipTests`.
- Run the test project directly with `dotnet test tests\MartinCostello.BrowserStack.Automate.Tests\MartinCostello.BrowserStack.Automate.Tests.csproj --configuration Release`.
- Run a single test with a filter and disable coverage, otherwise the test project's coverage threshold can fail partial runs. Example: `dotnet test tests\MartinCostello.BrowserStack.Automate.Tests\MartinCostello.BrowserStack.Automate.Tests.csproj --configuration Release --filter "FullyQualifiedName~MartinCostello.BrowserStack.Automate.AssemblyTests.Library_Is_Strong_Named" --property:CollectCoverage=false`.
- CI linting is defined in `.github\workflows\lint.yml`. Relevant local equivalents are:
  - `Invoke-ScriptAnalyzer -Path . -Recurse` for PowerShell.
  - `actionlint` for GitHub Actions workflows.
  - `zizmor .github\workflows` for workflow security linting.
  - `markdownlint-cli2 "**/*.md"` using `.markdownlint.json`.

## Architecture

- This repository contains one library project in `src\MartinCostello.BrowserStack.Automate` and one test project in `tests\MartinCostello.BrowserStack.Automate.Tests`.
- The library is a multi-targeted BrowserStack Automate REST API client (`net10.0`, `net8.0`, `netstandard2.0`). The main entry point is `BrowserStackAutomateClient`, which wraps `HttpClient`, sets BrowserStack basic auth, and exposes async methods for plans, browsers, builds, projects, sessions, logs, and destructive operations.
- `BrowserStackAutomateClient` owns the `HttpClient` only when it creates it internally. The overload that accepts an `HttpClient` is the DI-friendly path and is exercised by tests.
- Most DTOs are thin API-shape models decorated with `JsonPropertyName` attributes. Serialization is centralized through the source-generated `AppJsonSerializerContext`, so if a new request/response type is serialized or deserialized, it usually also needs to be added there.
- Some endpoints deserialize through internal transport wrappers rather than directly into the public models. For example, build and session list/detail endpoints use `AutomationBuild`, `AutomationSession`, `AutomationSessionDetail`, and `ProjectDetailItem` to unwrap nested API payloads before returning the public `Build`, `Session`, or `ProjectDetail` data.
- `BrowserStackAutomateClientExtensions` is intentionally small and only provides convenience wrappers around the core client, such as setting session status to completed or error.
- Tests are xUnit v3 + Shouldly. They are largely integration-style tests against the real BrowserStack API; credentials come from environment variables or user secrets (`BrowserStack_UserName`, `BrowserStack_AccessKey`). Missing credentials cause tests to skip rather than fail.

## Repository-specific conventions

- Public API compatibility is tracked in `src\MartinCostello.BrowserStack.Automate\PublicAPI\`. If you change the public surface, update the shipped/unshipped API files to match.
- Keep XML documentation comments and the standard Apache 2.0 file header on C# files. StyleCop and repository settings enforce documentation broadly, including many non-public members.
- Follow the existing C# style from `.editorconfig`: file-scoped namespaces, `var` preferred in all cases, braces on new lines, and expression-bodied properties/accessors but not methods/constructors.
- When adding client operations, follow the existing pattern in `BrowserStackAutomateClient`: validate inputs up front, keep methods async, thread `CancellationToken` through to `HttpClient`, and route JSON reads/writes through `GetJsonAsync()`/`PutJsonAsync()` with `JsonTypeInfo` from `AppJsonSerializerContext`.
- Preserve the existing API error behavior. `EnsureSuccessAsync()` translates BrowserStack JSON 404 payloads into `BrowserStackAutomateException`, while log endpoints intentionally return an empty string on 404 because BrowserStack returns HTML for missing logs.
- Tests use `TestContext.Current.CancellationToken` and Shouldly assertions. Match the existing Arrange/Act/Assert structure when adding tests.
- Destructive live tests are marked with `Fact(Explicit = true)` in `BrowserStackAutomateClientTests.cs`. Do not make destructive BrowserStack operations part of the default test flow.
- If you add a new client method, add tests in the matching `*Tests.cs` file for argument validation and any special exception behavior, not just the happy path.
- The repository uses centralized package management in `Directory.Packages.props` and shared build settings in `Directory.Build.props`; prefer updating those shared files instead of adding per-project version drift.

## General guidelines

- Always ensure code compiles with no warnings or errors and tests pass locally before pushing changes.
- Do not change the public API unless specifically requested.
- Do not use APIs marked with `[Obsolete]`.
- Bug fixes should **always** include a test that would fail without the corresponding fix.
- Do not introduce new dependencies unless specifically requested.
- Do not update existing dependencies unless specifically requested.
