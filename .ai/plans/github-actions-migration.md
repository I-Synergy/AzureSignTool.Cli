# Migrate Azure Pipelines to GitHub Actions

**Status:** DONE (see `.ai/completed/github-actions-migration.md`)
Started: 2026-10-27

## Goal

Replace `azure-pipelines.yml` with a GitHub Actions workflow that reproduces the
build → test → pack → sign → publish → tag/release flow, shaped like the
`I-Synergy.Framework` CI.

## Decisions

- `azure-pipelines.yml`: delete.
- Release trigger: every push to `main` (parity); PRs build/test only.
- Package signing: keep, using OIDC + Key Vault, skipped when `AZURE_KEYVAULT_URI` is unset.
- Workflow shape: `.github/workflows/ci.yml`, `windows-2025-vs2026`, NuGet trusted
  publishing, `gh release create --generate-notes`, matching the framework CI.

## Steps

- [x] Add `.github/workflows/ci.yml` (build + release jobs)
- [x] Make `Set-VersionVariables.ps1` and `New-GitTag.ps1` host-agnostic
- [x] Delete `azure-pipelines.yml`, `scripts/New-ReleaseNotes.ps1`, `codecoverage.runsettings`; update `.slnx`
- [x] Build, pack, verify and test exactly as the workflow does
