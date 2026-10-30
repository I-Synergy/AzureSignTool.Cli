# SonarCloud Cleanup

Status: DONE
Started: 2026-10-30

## Scope

SonarCloud project `I-Synergy_AzureSignTool.Cli` reports 1306 open issues at revision
`f29f95c`. Of those, 1204 are in `.ai/**` (template skills, example projects and vendored
Hugo themes) and are excluded from analysis. The remaining 102 findings in product code,
CI and scripts are fixed or, for Win32 interop naming/unsafe rules, suppressed with a
justification. The only product code-duplication block (`SipExtensionFactory.cs`, 64.2%)
is removed.

## Steps

- [x] Add `.sonarcloud.properties` exclusions for `.ai/**` and build output
- [x] Refactor `SipExtensionFactory` to remove duplicated extension branches
- [x] Fix CLI findings (`SignCommand`, configuration discoverer, `Program`, `ErrorOr`)
- [x] Fix Core findings (services, store, interop, enums, structures)
- [x] Fix CI workflow, PowerShell script and test assertion findings
- [x] Build in Release with MSBuild node reuse disabled and run all tests
- [x] Review the diff and record the final issue mapping
