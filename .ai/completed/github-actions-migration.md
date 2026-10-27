# Migrate Azure Pipelines to GitHub Actions

Status: DONE
Started: 2026-10-27
Completed: 2026-10-27

## Goal

Replace `azure-pipelines.yml` with a GitHub Actions workflow that reproduces the current
build → test → pack → sign → publish → tag/release flow, shaped like the
`I-Synergy.Framework` CI (`.github/workflows/ci.yml`).

## Decisions

| Decision | Choice |
|---|---|
| `azure-pipelines.yml` | Deleted |
| Release trigger | Every push to `main` (parity with today); PRs build/test only |
| Package signing | Kept, using the framework's OIDC + Key Vault approach, skipped when `AZURE_KEYVAULT_URI` is unset |
| Workflow shape | Same conventions as `I-Synergy.Framework`: `ci.yml`, `windows-2025-vs2026`, OIDC login, NuGet trusted publishing, `gh release create --generate-notes` |

## Mapping

| Azure Pipelines | GitHub Actions |
|---|---|
| `trigger`/`pr` on `main`, excluding `docs/*`, `*.md` | `on.push` / `on.pull_request`, `paths-ignore` |
| `$(Date:yyyy).1$(Date:MMdd).1$(Date:HHmm)` build number | computed in a `pwsh` step → `GITHUB_OUTPUT` |
| `Set-VersionVariables.ps1` + `##vso[task.setvariable]` | same script, dual-mode: writes `$GITHUB_OUTPUT` (falls back to the VSO command) |
| `Assembly-Info-NetCore@3` | MSBuild properties (`-p:Version`, `-p:PackageVersion`, `-p:AssemblyVersion`, `-p:FileVersion`, `-p:InformationalVersion`) |
| `UseDotNet@2` + `global.json` | `actions/setup-dotnet@v4` with `global-json-file` and package cache |
| `MSBuild@1 -t:restore,build,pack` | `dotnet restore` → `dotnet build` → `dotnet pack -o packages` |
| `DotNetCoreCLI@2 test` + `Code Coverage` runsettings | `dotnet test --collect:"XPlat Code Coverage"` (coverlet, opencover) |
| `PublishBuildArtifacts@1` | `actions/upload-artifact@v4` (`nupkgs`, `test-results`) |
| `CmdLine@2 sign` + variable group | `azure/login@v2` (OIDC) + `AzureSignTool sign -act WorkloadIdentityCredential` |
| `deployment` + `environment: $(Environment)` | `environment: production` |
| `NuGetCommand@2 push` + `Nuget.org` service connection | `NuGet/login@v1` trusted publishing + `dotnet nuget push --skip-duplicate` |
| `GitHubRelease@1` + `New-ReleaseNotes.ps1` | `New-GitTag.ps1` + `gh release create --generate-notes --verify-tag` |

## Steps

- [x] Add `.github/workflows/ci.yml` (build + release jobs)
- [x] Make `Set-VersionVariables.ps1` and `New-GitTag.ps1` host-agnostic
- [x] Delete `azure-pipelines.yml`, `scripts/New-ReleaseNotes.ps1`, `codecoverage.runsettings`; update the `.slnx` solution-items entry
- [x] Build, pack, verify and test exactly as the workflow does

## Notes

- Repository variables: `BUILD_CONFIGURATION` (optional), `AZURE_CLIENT_ID`,
  `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `AZURE_KEYVAULT_URI`,
  `AZURE_SIGNING_CERTIFICATE`, `TIMESTAMP_URL`. Secret: `NUGET_USER`.
  `scripts/Set-GitHubSettings.ps1` creates all of them (and the `production` environment)
  through an authenticated `gh` CLI.
- The `production` environment carries the approval gate that `$(Environment)` used to.
- Signing installs `AzureSignTool.Cli` from nuget.org, so that package must be published
  before a signed release can run.
- Verification: restore/build/pack for `Release` with the CI version properties succeeded;
  the package contains the `tools/` payload; `dotnet test -c Release --collect:"XPlat Code
  Coverage"` passed 49 CLI + 87 Core; `Set-VersionVariables.ps1` emitted the expected
  `GITHUB_OUTPUT` lines.

## Cross-platform runners (2026-10-27)

- `build` (restore/build/pack/test) runs on `ubuntu-latest`. Everything it needs is
  platform-neutral: the PowerShell steps run under `pwsh`, and `scripts/` use `git` only.
- Only the native signing path is Windows-bound (`mssign32`/`crypt32` P/Invokes), so:
  - `Program.Main` no longer refuses to start outside Windows; the guard moved into
    `SignCommand.RunSignAsync` *after* argument validation. `--help`, `--version` and all
    validation errors therefore work on any OS, and the CLI tests exercise them on Linux.
  - `CodeSigningServiceTests` and `MemoryCertificateStoreTests` carry
    `[OSCondition(OperatingSystems.Windows)]` and are skipped elsewhere.
- `release` (install AzureSignTool, sign, push) stays on `windows-2025`, because the tool
  itself is Windows-only.


## PR preview releases (2026-10-27, later same day)

- The `release` job now runs for `main` **and** for a pull request whose head branch lives in this
  repository (`github.event.pull_request.head.repo.full_name == github.repository`); fork PRs never
  publish because they receive no secrets.
- `environment:` is `acceptance` for a PR and `production` for `main`, mirroring the
  `I-Synergy.Framework` CI. Tag and GitHub-release steps are gated with
  `if: github.event_name != 'pull_request'`.
- PR packages carry the `-preview` suffix from `Set-VersionVariables.ps1`, so they cannot collide
  with a release version; `--skip-duplicate` covers a re-run.
- nuget.org trusted publishing pins the environment name, so a second policy for `acceptance` is
  required on nuget.org; the existing `production` policy covers releases only.
- `scripts/Set-GitHubSettings.ps1` now creates both environments (`-Environments`). Creating an
  environment does not add protection rules — required reviewers must be set in
  Settings → Environments for either gate to mean anything.
