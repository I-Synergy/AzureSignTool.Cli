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
- The next SonarCloud automatic analysis on push will confirm the dashboard.
  `Directory.Packages.props` still carries the pre-existing MSTest 4.5.2 bump.
- Committed as `2f4b944` on `development/main`; PR #2 targets `main`.
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
- Committed as `2f4b944` on `development/main`; PR #2 targets `main`.
- PR #2 follow-up: removed the nested ternary flagged by `csharpsquid:S3358` in
  `SignCommand`; Release build and all 139 tests pass.
- Both READMEs now carry the six SonarCloud badges (quality gate, bugs, code smells,
  vulnerabilities, security rating, maintainability rating).
- The CI badge line also carries the NuGet version and download badges.
- Record: `.ai/completed/release-versioning.md`; plan: `.ai/plans/release-versioning.md`.

## PR publishing disabled (2026-10-30, Reasonix Code)

- The `release` job now runs only for `refs/heads/main`; pull requests build, test and pack but
  never sign, push to nuget.org, tag, or create a release.
- `production` is the only environment `scripts/Set-GitHubSettings.ps1` creates, and both READMEs
  document that PRs are build-only.
- Reason: the nuget.org trusted-publishing policy is bound to `production`; PR preview publishing
  through `acceptance` failed with an environment mismatch.

## Five new SonarCloud issues fixed (2026-10-30, Reasonix Code)

- Queried `api/issues/search` for `I-Synergy_AzureSignTool.Cli` (new code period, unresolved):
  2 × `githubactions` vulnerabilities, 3 × `csharpsquid` code smells.
- `validate-template.yml`: dropped the unpinned `python -m pip install --upgrade pip`
  line, which triggered both workflow findings (`S8541` no `--only-binary :all:`,
  `S8544` unlocked dependency). The remaining install already pins versions and uses
  `--only-binary :all:`.
- `CodeSigningService.Dispose()` and `NuGetPackageSignatureService.Dispose()` now call
  `GC.SuppressFinalize(this)` (`S3881`).
- `SignCommand`: log placeholder `{thumbprint}` → `{Thumbprint}` (`S6678`).
- Verification: Release build 0 warnings/0 errors; 52 CLI + 87 Core tests pass;
  `git diff --check` clean. Awaiting the next SonarCloud analysis to confirm zero new issues.
- Committed as `11559da` on `fix/sonarcloud-five-issues`; PR #3 targets `main`.

## .NET 11 upgrade — CI audit (2026-11-11, Reasonix Code)

- CI had two .NET 10 leftovers that would have broken the pipeline: the `signing-tool` artifact
  path (`bin/<config>/net10.0`, with `if-no-files-found: error` → build job fails) and the release
  job's `dotnet-version: '10.0.x'` (it runs the net11.0 signing tool). Both now `net11.0` / `11.0.x`.
- Dropping the `System.Security.Cryptography.Pkcs` reference silently resolved it to 8.0.1
  transitively via `NuGet.Packaging 8.0.0` — re-pinned at 11.0.0 in `Directory.Packages.props`
  plus an explicit `PackageReference` in `AzureSignTool.Core`.
- Docs aligned: both readmes (SDK 11.0.100), `docs/WALKTHROUGH.md` (`11.0.x`),
  `.ai/project/tech-stack.md` (SDK, TFM, all package versions).
- Verification: Release build 0 warnings/0 errors; 52 CLI + 87 Core tests pass on net11.0;
  no direct package outdated; package contains only `tools/net11.0/any`.
- Left alone: `Directory.Build.props` `LangVersion 14.0` (builds clean at 15.0, but that is a
  code-semantics decision). Details: `.ai/completed/dotnet11-ci-audit.md`.

## Self-contained tool packaging (2026-11-11, Reasonix Code)

- The tool was framework-dependent, so consumers needed the matching .NET runtime. It now packs
  **self-contained per RID** (`RuntimeIdentifiers=win-x64;win-arm64;linux-x64`,
  `SelfContained` conditioned on `RuntimeIdentifier`), producing `AzureSignTool.Cli.<rid>`
  packages plus a small pointer package that NuGet resolves by machine RID.
- Consumers need a .NET 10+ CLI to install (RID tool packages are an SDK 10 feature) but **no .NET
  runtime** — verified: installed shim points at `tools/any/<rid>/AzureSignTool.exe`, runtimeconfig
  has `includedFrameworks`, works with an SDK 10.0.404 install client.
- `GeneratePackageOnBuild=false` on the CLI project (a build-time package cannot do the per-RID
  publishes); pointer package gets no `.snupkg` (NU5017 — empty symbols package).
- CI: Pack no longer uses `--no-build`; Verify packages asserts the 4-package shape and the
  bundled runtime; the signing tool is staged with `dotnet publish --no-build -o signing-tool`
  (the old `bin/.../net11.0` path would have uploaded ~250 MB of RID publishes); pushes now send
  RID packages **before** the pointer package.
- Verification: Release build 0 warnings/0 errors; 52 CLI + 87 Core tests pass; `dotnet pack`
  produces pointer + win-x64/win-arm64/linux-x64 (39–41 MB each) + 3 snupkgs; the workflow's
  verify script passes and throws on a pointer-only folder.
- Record: `.ai/completed/self-contained-tool-packaging.md`.

## Release job no longer installs .NET (2026-11-11, Reasonix Code)

- The `Install .NET SDK 11.0` step in the release job existed only because the signing tool was
  run framework-dependently (`dotnet signing-tool/AzureSignTool.dll`). With the tool now packed
  self-contained per RID, the release job unpacks `AzureSignTool.Cli.win-x64.*.nupkg` from the
  `nupkgs` artifact and runs `tools/any/win-x64/AzureSignTool.exe` directly — no runtime, and it
  signs with the exact binary that ships.
- Removed `setup-dotnet` from the release job and the `Stage signing tool` / `Upload signing tool`
  steps from the build job. The only remaining .NET use is `dotnet nuget push`, provided by the
  `windows-2025` image.
- Verified: the extraction step, taken from the workflow, picks exactly the win-x64 nupkg out of
  the four artifacts and yields `tools/any/win-x64/AzureSignTool.exe`; the extracted
  self-contained binary runs `--version` and `sign --help` with no `dotnet` involved.
