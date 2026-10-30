# Session Handoff - 2026-10-27

**Written By:** Reasonix Code
**Next Session May Use:** Claude Code | Copilot | Reasonix Code | Any

## Task Summary

Two related changes landed in one session:

1. **Package/project rename** — `I-Synergy.Tools.Azure.SignTool` → `AzureSignTool.Cli`.
   Solution `AzureSignTool.Cli.slnx`; projects `src/AzureSignTool.Cli` and
   `src/AzureSignTool.Core`; tests `tests/AzureSignTool.Cli.Tests` (49 tests) and
   `tests/AzureSignTool.Core.Tests` (87 tests); namespaces `AzureSignTool.Cli.*` /
   `AzureSignTool.Core.*`. `AssemblyName` and `ToolCommandName` remain `AzureSignTool`.
   Strong-name signing was dropped (`SignAssembly`/`AssemblyOriginatorKeyFile` removed, `.snk`
   deleted), which also reduced `InternalsVisibleTo` to `AzureSignTool.Core.Tests`.
2. **CI moved from Azure DevOps to GitHub Actions** — `.github/workflows/ci.yml` replaces
   `azure-pipelines.yml` (deleted). `build` job runs on `ubuntu-latest`; `release` job
   (sign + nuget.org push + tag + GitHub release) runs on `windows-2025`.

## Decisions Made

- **Delete `azure-pipelines.yml`**: single CI host. Alternatives: keep as fallback (rejected —
  two sources of truth).
- **Release on every push to `main`** (parity with the old pipeline); PRs only build.
- **Build/test on Linux**: nothing in the build is Windows-specific; signing is. The platform
  guard moved from `Program.Main` into `SignCommand.RunSignAsync` (after validation) so help,
  version and validation work everywhere; native tests are gated with
  `[OSCondition(OperatingSystems.Windows)]`.
- **GitHub shape mirrors `I-Synergy.Framework`'s `ci.yml`**: OIDC `azure/login`, NuGet trusted
  publishing (`NuGet/login@v1`, `NUGET_USER`), `gh release create --generate-notes`,
  `BUILD_CONFIGURATION` variable, `GeneratePackageOnBuild=false` + explicit `dotnet pack`.
- **Signing stays on Windows** because AzureSignTool P/Invokes `mssign32`/`crypt32`.

## Open Questions

- **OPEN**: Is the release job supposed to keep using the published `AzureSignTool.Cli` tool for
  signing? It creates a bootstrap problem for the very first release of the renamed package.
- **ASSUMED**: The `windows-2025` runner label and NuGet trusted-publishing policy (workflow
  `ci.yml`, environment `production`) match the org setup; verify on the first run.
- **OPEN**: Root `README.md` is still the AI-template overview (untracked, copied from
  `I-Synergy/AI`). Once committed it shadows `.github/readme.md`, which is the project README
  GitHub currently resolves. Swap them if the template overview is not wanted here.

## Files Modified

- `.github/workflows/ci.yml` — created (build + release jobs)
- `azure-pipelines.yml`, `scripts/New-ReleaseNotes.ps1`, `codecoverage.runsettings` — deleted
- `scripts/Set-VersionVariables.ps1`, `scripts/New-GitTag.ps1` — dual-mode GitHub/ADO output
- `scripts/Set-GitHubSettings.ps1` — created; sets repository variables + `NUGET_USER` secret
  and the `production` environment through `gh`
- `src/AzureSignTool.Cli/Program.cs` — platform guard removed from `Main`
- `src/AzureSignTool.Cli/Commands/SignCommand.cs` — guard added in `RunSignAsync`
- `tests/AzureSignTool.Core.Tests/Services/CodeSigningServiceTests.cs`,
  `tests/AzureSignTool.Core.Tests/Stores/MemoryCertificateStoreTests.cs` — `[OSCondition]`
- `AzureSignTool.Cli.slnx` — solution items point at `.github/workflows/ci.yml`
- Docs: `.github/readme.md`, `src/AzureSignTool.Cli/readme.md` (CI section + badge),
  `docs/WALKTHROUGH.md` (GitHub Actions section), `docs/SUPPORT.md`,
  `tests/AzureSignTool.Cli.Tests/TEST_SUMMARY.md`, `.ai/project/tech-stack.md`,
  `.ai/project/architecture.md`, `AGENTS.md`, `.github/copilot-instructions.md`

## Conditional Branches

- If `AZURE_KEYVAULT_URI` is unset: the release job skips `azure/login` and signing, and pushes
  unsigned packages.
- If the workflow runs on a PR: only `build` runs; packages carry a `-preview` suffix.
- If a rename happens again: `NuGet/login` trusted publishing is bound to the workflow file name
  `ci.yml` and environment `production`; update the policy on nuget.org.

## Cross-Document Relationships

- `.github/workflows/ci.yml` reads the variables/secrets documented in its header and created by
  `scripts/Set-GitHubSettings.ps1`.
- `scripts/Set-VersionVariables.ps1` writes `GITHUB_OUTPUT` consumed by the build steps in
  `ci.yml`; `scripts/New-GitTag.ps1` creates the tag that `gh release create` verifies.
- `.ai/project/tech-stack.md` and `.ai/project/architecture.md` describe the same CI split.

## Next Actions (Priority Order)

1. Run the workflow once on GitHub (`workflow_dispatch`) and confirm the Ubuntu job, the signing
   skip/execute path, and the release job's tag + release.
2. Commit the rename + CI changes (only `AzureSignTool.Cli.slnx`, `.github/workflows/`, `scripts/`,
   `src/`, `tests/`, docs) — the working tree is currently entirely uncommitted.
3. Decide on the root `README.md` template file (keep/shadow/swap).

## Files to Load Next Session

- `.ai/completed/github-actions-migration.md` — full migration record and verification evidence
- `.ai/completed/rename-azuresigntool-cli.md` — rename record
- `.github/workflows/ci.yml` — current build/release definition

## Work Type for Next Session

CI/CD verification | Release

## Reason for Session End

- Task complete (documentation update)

## Session Notes

- Verification on this machine: Release build succeeds; `dotnet test` → 49 + 87 passed; the
  package contains the `tools/` payload; `Set-VersionVariables.ps1` emits the expected
  `GITHUB_OUTPUT` lines. The Ubuntu leg cannot be executed locally (no Docker/WSL runner).
- `.gitignore` was restored to ignore `bin/`, `obj/`, `packages/` (the template's minimal
  `.gitignore` had dropped them).

## verify-config Audit (2026-10-27, Reasonix Code)

- Ran `.ai/skills/verify-config/SKILL.md`. Fixed: progress files now explicitly MANDATORY in
  `AGENTS.md` and `.github/copilot-instructions.md`; `.ai/reference/task-execution.md` made
  tool-agnostic and its stale `programmer` agent renamed to `developer`.
- Session context stays a project handoff (this is a product repo, not the template placeholder).
- Generated `COMPLIANCE.md` and the marked `README.md` standards block. `validate-standards.py`
  still exits 1 on TEST 6: ISO/IEC/IEEE 29119-3:2021 declares `Evidence`, computes `Aligned`
  because `docs/slices/*/test-plan.md` and `docs/test-reports/*` do not exist. Both surfaces carry
  the downgrade note; the Evidence group reads *(none)*.
- Root `README.md` is now the project readme (template overview preserved as
  `TEMPLATE-OVERVIEW.md`); `.github/readme.md` was removed.
- Full record: `.ai/completed/verify-config-audit.md`.

## README Verification (2026-10-27, Reasonix Code)

- `README.md` and the packaged `src/AzureSignTool.Cli/readme.md` verified for links, names and
  commands. Fixed: CI link path, LICENSE link, TOC entry/heading anchor, requirements, exit codes,
  install note, `--colors` cell, explicit `-act ClientSecretCredential` in examples.
- Found and fixed a CLI validation contradiction: `-act ClientSecretCredential`/`AccessTokenCredential`
  could never satisfy validation because their own parameters counted as competing mechanisms.
  `SignCommand.ValidateArguments` now allows the parameters a credential type consumes; three tests
  added (52 CLI tests total).
- Record: `.ai/completed/readme-verification.md`.

## PR Preview Publishing (2026-10-27, Reasonix Code)

- `release` job now also runs for same-repo PRs through the `acceptance` environment and publishes
  the `-preview` package; `main` still publishes through `production` and creates the tag/release.
- Requires a second nuget.org trusted-publishing policy bound to `acceptance`.
- `scripts/Set-GitHubSettings.ps1` creates both environments; reviewers must be added manually.

## NuGet trusted publishing username (2026-10-27, Reasonix Code)

- The `NUGET_USER` secret must be the nuget.org account `ismailhassani`, not the `I-Synergy`
  organization. `scripts/Set-GitHubSettings.ps1` now defaults `-NuGetUser` to `ismailhassani`, and
  the workflow header plus both readmes name that account.
- The nuget.org policies themselves are managed outside the repository; one policy per environment
  (`production`, `acceptance`).

## GitHub settings automation (2026-10-27, Reasonix Code)

- GitHub currently has no environments/variables/secrets for this repo; nothing creates them
  automatically. `scripts/Set-GitHubSettings.ps1` now also configures required reviewers on
  `production` and `acceptance` (`-Reviewers`, default `ismailhassani`).
- The release job fails fast when `NUGET_USER` is missing and warns when signing is skipped
  (`AZURE_KEYVAULT_URI` unset).

## GitHub + Azure configuration applied (2026-10-27, Reasonix Code)

Applied through the GitHub REST API (token from Git Credential Manager) and `az` CLI:

- Repository variables: `BUILD_CONFIGURATION=Release`, `AZURE_CLIENT_ID=6e6a6823-762e-4893-96d5-bec13a76df06`
  (`DevOpsCodeSigning`), `AZURE_TENANT_ID=33ab98f7-f500-4bde-b23f-8a0625aad6ac`,
  `AZURE_SUBSCRIPTION_ID=87a91c6d-c7ae-46ae-99bf-c164b75e1cde`,
  `AZURE_KEYVAULT_URI=https://i-synergy.vault.azure.net/`,
  `AZURE_SIGNING_CERTIFICATE=sslcom-code-signing-certificate`,
  `TIMESTAMP_URL=http://timestamp.digicert.com` (mirrors the I-Synergy.Framework variables).
- Repository secret `NUGET_USER=ismailhassani`.
- Environments: `production` gated by required reviewer IsmailHassani (self-review allowed);
  `acceptance` deliberately ungated so PR previews publish directly from CI.
- Federated credentials `github-actions-azuresigntoolcli-production` /
  `-acceptance` added to app `DevOpsCodeSigning` for
  `repo:I-Synergy/AzureSignTool.Cli:environment:{production,acceptance}`.
- Signing app's Key Vault access policy already grants keys Sign/Get and certificates Get/List;
  certificate `sslcom-code-signing-certificate` expires 2029-01-29.

## Mirror removed, warnings fixed, main is live (2026-10-27, Reasonix Code)

- `.github/workflows/mirror-to-azure.yml` deleted (no Azure DevOps repo exists); the mirroring
  section and tree entry in `TEMPLATE-OVERVIEW.md` removed with it.
- `CreateManagedIdentityCredential` now uses `ManagedIdentityCredentialOptions(ManagedIdentityId)`
  — `ManagedIdentityCredential(string, TokenCredentialOptions)` was obsolete.
- README acknowledgement link simplified; the nested-parenthesis link made
  `.ai/tests/validate-references.sh` fail.
- Pushed to `main`; CI build job green, release job waiting for the `production` approval.
- `Validate Claude Template` still fails, only at `validate-standards.py`: row
  ISO/IEC/IEEE 29119-3:2021 declares `Evidence`, the project produces none of the
  `docs/slices/*` test plans or `docs/test-reports/*`. All other 10 suites pass locally.

## SonarCloud cleanup (2026-10-30, Reasonix Code)

- Baseline: 1306 open issues at revision `f29f95c` — 1204 under `.ai/**` and 102 in
  product/CI/scripts. Product duplication: one block in `SipExtensionFactory.cs` (64.2%).
- Added `.sonarcloud.properties` excluding `.ai/**` and build output from SonarCloud
  analysis; this is the agreed treatment for template skills, examples and vendored Hugo
  themes rather than rewriting 1204 findings in non-shipped assets.
- Fixed the 102 project findings across `SignCommand`, `AzureKeyVaultConfigurationDiscoverer`,
  `Program`, `ErrorOr`, `CodeSigningService`, `NuGetPackageSignatureService`,
  `NuGetPackageSigner`, `MemoryCertificateStore`, CI workflows and PowerShell scripts.
- Win32 interop names (`HRESULT`, `crypt32`, `mssign32`, native structs/enums) and unsafe
  blocks are retained and suppressed with justifications, per user decision; no public API
  rename was made.
- `SipExtensionFactory` now uses extension sets, removing its duplicated branches.
- Verification: Release build 0 warnings/0 errors; 52 CLI + 87 Core tests pass; both
  PowerShell scripts parse; `git diff --check` clean.
- Not committed or pushed. The next SonarCloud automatic analysis on push will confirm the
  dashboard. `Directory.Packages.props` still carries the pre-existing MSTest 4.5.2 bump.
- Record: `.ai/completed/sonarcloud-cleanup.md`; plan: `.ai/plans/sonarcloud-cleanup.md`.

## Automatic release versioning aligned with AI (2026-10-30, Reasonix Code)

- Adopted the `I-Synergy/AI` release scheme: build number `YYYY.1MMdd.1HHmm` in UTC, tag and
  GitHub release use that number directly (no `v` prefix), and no project file is bumped.
- `.github/workflows/ci.yml` now accepts an optional `workflow_dispatch` `build_number`,
  validates the format, and skips tag/release creation when the tag already exists.
- `scripts/New-GitTag.ps1` no longer prefixes `v`; `Set-VersionVariables.ps1` pads PR build
  seconds to two digits (matching the AI workflow).
- Removed `Version`, `AssemblyVersion`, and `FileVersion` pins from `Directory.Build.props`;
  CI remains the single source of truth and passes all version properties to build/pack.
- Updated both READMEs to document the unprefixed tag and no-bump behaviour.
- Verification: merge version `2026.11030.12345.0`, PR preview `...36-preview`; versionless and
  CI-versioned Release builds succeed; 52 CLI + 87 Core tests pass; workflow YAML and PowerShell
  parse; `git diff --check` clean.
- Record: `.ai/completed/release-versioning.md`; plan: `.ai/plans/release-versioning.md`.
