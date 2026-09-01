# AGENTS.md

## What this repo is
A personal workspace of **independent ASP.NET Core Web API practice projects** (course exercises / deep dives). There is no single app: each top-level folder (and `NET6/`, `api_templates/`) is its own project or solution. Treat them as separate units — never build the whole repo at once and assume a clean result.

**Not everything compiles.** Several projects are mid-exercise and currently fail to build (e.g. `EmployeeManagement.Test/InternalEmployeesIntegrationTests.cs` has a stray extra `}`; `Books.API/Controllers/BooksController.cs` has a `Guid` → `string` argument error; the `NET6/` tree has duplicate AutoMapper profile classes). Verify the specific project you touch rather than assuming the tree is green.

## Two target generations
- **`net10.0`** — the active projects at the repo root: `Books.API`, `BookCovers.API`, `Books.Legacy`, `CityInfo.API`, `CourseLibrary.API`, `DishesAPI`, `DishesAPI_deepdive`, `EmployeeManagement`(+`.Test`), `Library.API`, `Library.Client`, `TopLevelManagement`.
- **`net6.0`** — legacy/snapshot trees that are generally not the focus: everything under `NET6/` and `api_templates/` (`controllers`, `fastendpoints`, `minimal`, `BackendData`, `APIProjectTests`, `ApiBestPractices.Endpoints`).

## Solutions (partial groupings; many overlap)
- `WebProjects.sln` — main `net10.0` web apps (classic `.sln` format)
- `Books.slnx`, `EmployeeManagement.slnx`, `Library.API.slnx` — `.slnx` (new XML solution format)
- `CourseLibrary.sln` (root, `net10.0`) vs `NET6/CourseLibrary.sln` (old) — two different targets, don't mix
- `ApiProjectTemplates.sln` — the `api_templates/` projects

## Commands
- Installed SDK: `10.0.400` (also `2.1`, `7.0`, `10.0.202`). There is **no .NET 6 SDK**; `net6.0` projects still restore/build via targeting packs, but the `NET6/` tree is broken anyway.
- Build / test a single unit:
  ```
  dotnet build Books.slnx
  dotnet build EmployeeManagement.slnx
  dotnet test Books.API.Test
  dotnet test EmployeeManagement.Test
  ```
- No `Directory.Build.props`, no `global.json`, no CI workflows, no lint/format script. Style is enforced only by the root `.editorconfig` (run `dotnet format` only if you want to, it is not invoked automatically).

## Library.Client ↔ Library.API codegen ordering
`Library.Client` is an **NSwag-generated client** (`NSwag.ApiDescription.Client`) that consumes `Library.API/openapispecifications/Library.API.json`, which `Library.API` emits at build time (`OpenApiGenerateDocument`). Build `Library.API.slnx` (the API first) before/with `Library.Client`; building the client alone can fail if the spec is stale, and changes to API shapes require regenerating that spec.

## EmployeeManagement (most active/testing area)
- Architecture: `Controllers/`, `Business/` (services + `EmployeeFactory`), `DataAccess/` (EF Core `EmployeeDbContext` + repository), `Models/`, `MapperProfiles/` (AutoMapper), `Middleware/`, `ActionFilters/`. DI wired in `Program.cs` + `ServiceRegistrationExtensions.cs`.
- DB: EF Core **SQLite** (`EmployeeManagement.db`), migrations under `Migrations/`.
- Tests use **xUnit v3** (`xunit.v3` 3.2.2) — note `TestContext.Current.CancellationToken` and `Assert.ThrowsAsync`; not the legacy xUnit 2 API.
- Integration tests (`*.IntegrationTests.cs`) use `WebApplicationFactory<Program>` via `CustomWebApplicationFactory` (`Fixtures/`), which swaps the real SQLite connection for an **in-memory SQLite `DataSource=:memory:` and calls `EnsureCreated()`** (not EF migrations). Auth is faked with a custom `TestAuthHandler` scheme (see `Fixtures/TestAuthHandler.cs`); controller-layer tests mock `IEmployeeService` with Moq and use a shared `EmployeeServiceCollection`/fixtures.
- `Books.API.Test` is the other test project but uses **xUnit 2.x** — different API style from EmployeeManagement.Test; don't assume one convention.

## Style (root `.editorconfig`)
- Tabs for indentation, `charset = utf-8-bom`, CRLF, final newline.
- Namespaces are **block-scoped** (matches the code; the `[*.cs]` section overrides the file-scoped rule).
- Instance fields are camelCase prefixed `_`; constants PascalCase.
- `csharp_prefer_braces`, `dotnet_sort_system_directives_first`.

## Gotchas
- `NET6/Books.API/Books - Backup.API.csproj` has a space and "Backup" in its name and is not part of any solution — ignore it.
- Some apps hardcode dev URLs in `Program.cs` (e.g. `Books.API` calls BookCovers at `https://localhost:52644`) instead of config, so running full multi-app flows locally requires matching launch ports.
- `BookCovers.API`/`Books.API` reference `SQLitePCLRaw.lib.e_sqlite3` `2.1.11`, which GitHub flags as a known-high-severity vuln (NU1903); `Library.API` and other projects emit `CS1591` (missing XML docs) warnings by design because `GenerateDocumentationFile` is on.
