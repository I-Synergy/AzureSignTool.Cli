# Rename to AzureSignTool.Cli

**Status:** DONE
Started: 2026-10-27
Completed: 2026-10-27

## Steps
- [x] Remove stale bin/obj and rename solution, project folders, csproj files
- [x] Rewrite namespaces, package IDs, project references, readme install commands
- [x] Update azure-pipelines.yml and docs
- [x] Fix InternalsVisibleTo after strong-name removal
- [x] Build, test, review diff

## Notes

- New identity: package ID `AzureSignTool.Cli`, projects `AzureSignTool.Cli` /
  `AzureSignTool.Core`, tests `AzureSignTool.Cli.Tests` / `AzureSignTool.Core.Tests`,
  solution `AzureSignTool.slnx`, namespaces `AzureSignTool.Cli.*` / `AzureSignTool.Core.*`.
- `AssemblyName` and `ToolCommandName` stay `AzureSignTool`, so the command line is unchanged.
- Repository URLs point at `https://github.com/I-Synergy/AzureSignTool.Cli`.
- `InternalsVisibleTo` was reduced to `AzureSignTool.Core.Tests` because strong-name signing
  was removed (fixes 15 × CS0281 that blocked the build before the rename).
- Verification: `dotnet build AzureSignTool.slnx` succeeded (0 errors),
  `dotnet test AzureSignTool.slnx` passed 49 CLI + 87 Core tests.
- Follow-up for release: publish one final `I-Synergy.Tools.Azure.SignTool` version,
  deprecate it with `AzureSignTool.Cli` as the alternate package, and add
  `AzureSignTool.slnx` to git (it is currently untracked while the old solution is deleted).

## Post-completion (2026-10-27)

- Solution was renamed again after this record was written: `AzureSignTool.slnx` →
  **`AzureSignTool.Cli.slnx`**. All references above that say `AzureSignTool.slnx` mean the
  current `AzureSignTool.Cli.slnx`.
- `azure-pipelines.yml` still pointed at `AzureSignTool.slnx` in the MSBuild `solution:` and
  DotNetCoreCLI `projects:` inputs; both now point at `AzureSignTool.Cli.slnx`.
- `.gitignore` had been replaced with the template's 21-line file, dropping the .NET build
  ignores; `bin/`, `obj/`, `packages/` and friends are ignored again.
