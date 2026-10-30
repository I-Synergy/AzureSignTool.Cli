# SonarCloud Cleanup

Status: DONE
Started: 2026-10-30

## Steps

- [x] Add `.sonarcloud.properties` exclusions for `.ai/**` and build output
- [x] Refactor `SipExtensionFactory` to remove duplicated extension branches
- [x] Fix CLI findings (`SignCommand`, configuration discoverer, `Program`, `ErrorOr`)
- [x] Fix Core findings (services, store, interop, enums, structures)
- [x] Fix CI workflow, PowerShell script and test assertion findings
- [x] Build in Release with MSBuild node reuse disabled and run all tests
- [x] Review the diff and record the final issue mapping

## Notes

- Baseline: 1306 open issues / 0.6% duplication at `f29f95c`.
- 1204 issues are under `.ai/**`; excluded by user decision.

## Verification

- `dotnet build AzureSignTool.Cli.slnx -c Release` — succeeded, 0 warnings, 0 errors.
- `dotnet test AzureSignTool.Cli.slnx -c Release --no-build` — 52 CLI + 87 Core tests passed.
- Both modified PowerShell scripts parse cleanly with the PowerShell AST parser.
- `git diff --check` — clean.

## Outcome

- `.ai/**` is excluded from SonarCloud analysis, removing 1204 template/vendored findings.
- All 102 project findings are fixed or suppressed with a Win32 justification.
- The only product duplication block (`SipExtensionFactory.cs`) is removed.
- The next SonarCloud automatic analysis after pushing this branch will confirm the dashboard.
