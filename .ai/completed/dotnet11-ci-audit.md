# .NET 11 CI Audit

Status: DONE
Started: 2026-11-11
**Status:** DONE

## Scope

Verify that the .NET 11 upgrade (global.json, all four project files, Central Package
Management entries) is reflected in CI and in every version-bearing document, and that no
older framework/NuGet version survives.

## Findings and fixes

| File | Problem | Fix |
|------|---------|-----|
| `.github/workflows/ci.yml:194` | `Upload signing tool` still pointed at `bin/<config>/net10.0`; with `if-no-files-found: error` the **build job would have failed** | path → `net11.0` |
| `.github/workflows/ci.yml:244-251` | release job installed SDK `10.0.x`; it runs the net11.0 signing tool, so **signing/publishing would have failed** | `11.0.x` + comment/step renamed to 11.0 |
| `Directory.Packages.props` / `AzureSignTool.Core.csproj` | dropping the `System.Security.Cryptography.Pkcs` reference let it resolve **transitively to 8.0.1** (from `NuGet.Packaging 8.0.0`) — a downgrade of the CMS/signature library from the previous 10.0.14 pin | re-pinned at `11.0.0` (direct reference restored) |
| `README.md:468`, `src/AzureSignTool.Cli/readme.md:466` | advertised SDK `10.0.102` | `11.0.100` |
| `docs/WALKTHROUGH.md:196` | user-facing signing snippet used `dotnet-version: '10.0.x'` | `11.0.x` |
| `.ai/project/tech-stack.md` | SDK `10.0.102`, TFM `net10.0`, System.CommandLine 2.0.14, Extensions 10.0.14, NuGet.Packaging 7.9.0, Pkcs 10.0.14, System.IO.Packaging 10.0.2, MSTest 4.5.1 | SDK `11.0.100`, `net11.0`, 3.0.0, 11.0.0, 8.0.0, Pkcs 11.0.0, removed the now-unreferenced System.IO.Packaging row, MSTest.TestAdapter 4.5.2 |

## Verified

- `dotnet restore` / `dotnet build -c Release` (SDK 11.0.100): succeeded, 0 warnings, 0 errors.
- `dotnet test -c Release --no-build`: 52 CLI + 87 Core passed on `net11.0`.
- `dotnet list package --outdated`: no direct package has a newer version.
- Resolved graph: `NuGet.Packaging 8.0.0`, `System.Security.Cryptography.Pkcs 11.0.0`.
- Packed tool contains `tools/net11.0/any/*` only.
- `ci.yml` parses; no `net10`/`10.0` reference remains in CI, docs or props.

## Outstanding (deliberately not changed)

- `Directory.Build.props` still pins `LangVersion 14.0`. SDK 11.0.100 accepts `15.0` and the
  solution builds clean at that level (probed), but the language version is a code-semantics
  choice, so it was left as-is.
- `global.json` keeps `allowPrerelease: true` and a `tools` section (harmless, unused by the SDK).
