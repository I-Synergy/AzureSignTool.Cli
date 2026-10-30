# Align Release Versioning With the AI Repository

Status: DONE
Started: 2026-10-30

## Steps

- [x] Align build-number derivation with the AI repo (UTC, validation, manual input)
- [x] Remove the `v` prefix from tags, releases, and `New-GitTag.ps1`
- [x] Skip tag/release creation when the version tag already exists
- [x] Remove manual version pins from `Directory.Build.props`
- [x] Update release documentation and record verification

## Notes

- Reference: `../AI/.github/workflows/ci.yml` and its README "Release Versioning" section.
- Existing `v...` tags remain; only new releases use the unprefixed build number.

## Verification

- `Set-VersionVariables.ps1` produced `2026.11030.12345.0` / `2026.11030.12345` for a merge build.
- The same script produced a padded PR build number (`...36`) and `...-preview` package version.
- Release builds succeed both without version properties and with CI-supplied version properties.
- Full test suite: 52 CLI + 87 Core tests passed.
- Both workflow YAML files parse with PyYAML.
- Both modified PowerShell scripts parse with the PowerShell AST parser.
- `git diff --check` — clean.
