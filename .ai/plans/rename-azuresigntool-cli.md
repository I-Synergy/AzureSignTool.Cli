# Rename tool identity to AzureSignTool.Cli

Status: DONE
Started: 2026-10-27

## Goal

Replace the `ISynergy.Tools.Azure.SignTool` identity everywhere with the
`AzureSignTool.Cli` identity, and publish under the NuGet package ID
`AzureSignTool.Cli`.

## Naming map

| Old | New |
|---|---|
| NuGet package ID `I-Synergy.Tools.Azure.SignTool` | `AzureSignTool.Cli` |
| Project `src/ISynergy.Tools.Azure.SignTool` | `src/AzureSignTool.Cli` |
| Project `src/ISynergy.Tools.Azure.SignTool.Core` | `src/AzureSignTool.Core` |
| Tests `ISynergy.Tools.Azure.SignTool.Tests` | `AzureSignTool.Cli.Tests` |
| Tests `ISynergy.Tools.Azure.SignTool.Core.Tests` | `AzureSignTool.Core.Tests` |
| Solution `Azure.SignTool.slnx` | `AzureSignTool.slnx` |
| Namespaces `ISynergy.Tools.Azure.SignTool.*` | `AzureSignTool.Cli.*` |
| Namespaces `ISynergy.Tools.Azure.SignTool.Core.*` | `AzureSignTool.Core.*` |
| `AssemblyName` / `ToolCommandName` | unchanged: `AzureSignTool` |
| GitHub repository URLs | unchanged: `I-Synergy/Azure.SignTool` |

## Steps

1. Remove stale `bin`/`obj` output and rename solution, project folders, csproj files.
2. Rewrite namespaces, package IDs, project references and readme install commands.
3. Update `azure-pipelines.yml` and docs.
4. Drop the now-invalid `PublicKey=` from `InternalsVisibleTo` (strong-name signing was removed in this working tree); this currently breaks the build with 15 × CS0281.
5. Build with node reuse disabled, run tests, review the diff.

## Notes

- NuGet has no in-place rename: `AzureSignTool.Cli` is a new package. Existing
  `I-Synergy.Tools.Azure.SignTool` consumers need to uninstall/reinstall; publish one
  final old-ID version and deprecate it with the new alternate package.
- Assembly/tool command remains `AzureSignTool`, so command-line usage does not change.
