# Align Release Versioning With the AI Repository

Status: DONE
Started: 2026-10-30

## Goal

Use the same automatic release versioning as `I-Synergy/AI`: the build number is
`YYYY.1MMdd.1HHmm` in UTC, the tag and GitHub release use that number with no `v` prefix,
and no project file needs a manual version bump. A manual `workflow_dispatch` run accepts an
optional build number.

## Steps

- [x] Align build-number derivation with the AI repo (UTC, validation, manual input)
- [x] Remove the `v` prefix from tags, releases, and `New-GitTag.ps1`
- [x] Skip tag/release creation when the version tag already exists
- [x] Remove manual version pins from `Directory.Build.props`
- [x] Update release documentation and record verification
