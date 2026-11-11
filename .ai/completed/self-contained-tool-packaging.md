# Self-Contained Tool Packaging

Status: DONE
Started: 2026-11-11
**Status:** DONE

## Problem

Installing `AzureSignTool.Cli` required a matching .NET runtime on the consumer machine: the tool
package was framework-dependent (`tools/net11.0/any`, `Runner="dotnet"`), so a net11.0 tool failed
on any machine without the .NET 11 runtime — including CI runners whose SDK was older.

## Decision

Publish the tool **self-contained per RID** using the .NET SDK's RID-specific tool packaging
(SDK 10+, `Microsoft.NET.PackTool.targets`):

- `RuntimeIdentifiers=win-x64;win-arm64;linux-x64` — each RID becomes `AzureSignTool.Cli.<rid>`.
- `SelfContained` only for RID builds (`Condition="'$(RuntimeIdentifier)' != ''"`), so ordinary
  `dotnet build` stays RID-agnostic and the release job's signing tool is still a plain
  framework-dependent layout.
- `AzureSignTool.Cli` itself becomes a small pointer package whose `DotnetToolSettings.xml` lists
  the `RuntimeIdentifierPackages`.

Rejected alternatives: NativeAOT and trimming (Azure.Identity and NuGet.Packaging/Newtonsoft
resolve types reflectively); standalone exe release assets (the user chose the NuGet tool path,
which keeps `dotnet tool install`).

`GeneratePackageOnBuild=false` had to be set for the CLI: a `dotnet build` -generated package
cannot run the per-RID publishes.

## Verified end to end

- `dotnet pack` → `AzureSignTool.Cli.1.0.0.nupkg` (pointer, 32 KB) +
  `.win-x64` (41 MB) + `.win-arm64` (39 MB) + `.linux-x64` (39 MB), each with a `.snupkg`.
- `dotnet tool install --tool-path … --add-source packages AzureSignTool.Cli` succeeds; the
  generated shim is
  `"…\.store\azuresigntool.cli\1.0.0\azuresigntool.cli.win-arm64\1.0.0\tools\any\win-arm64\AzureSignTool.exe" %*`
  — no `dotnet` in the path.
- Bundled `AzureSignTool.runtimeconfig.json` declares `includedFrameworks` (Microsoft.NETCore.App
  11.0.0), 245 files in the RID folder; `--version` and `sign --help` run.
- Install with an **SDK 10.0.404** client (global.json-pinned) also succeeds and runs, so the
  minimum install client is .NET 10, not .NET 11.
- A host RID that is not packaged fails fast with
  `The tool does not support the current architecture or operating system`.
- Release build 0 warnings/0 errors; 52 CLI + 87 Core tests pass; `dotnet pack` exits 0.

## CI changes (`.github/workflows/ci.yml`)

- **Pack**: dropped `--no-build` — RID packages require the per-RID publish.
- **Verify packages**: now asserts the whole shape — exactly one pointer package, 4 packages
  total, pointer declares all three RIDs, each RID package has
  `tools/any/<rid>/DotnetToolSettings.xml`, bundles `System.Private.CoreLib.dll`, and its
  runtimeconfig has `includedFrameworks` (no `"framework"`). Guards against silently reverting to
  a framework-dependent package. Negative path checked (throws on a pointer-only folder).
- **Signing tool**: no `signing-tool` artifact is produced any more. The release job unpacks
  `AzureSignTool.Cli.win-x64.*.nupkg` (already downloaded for publishing) into `signing-tool/` and
  runs `tools/any/win-x64/AzureSignTool.exe`, so it signs with the exact self-contained binary
  that ships and needs no .NET runtime.
- **No `setup-dotnet` in the release job**: with the signing tool self-contained, the only .NET
  use left is `dotnet nuget push`, which the `windows-2025` image provides. The job no longer
  installs an SDK (previously `11.0.x`, only to host a net11.0 framework-dependent signing tool).
- **Push to NuGet.org**: RID packages are pushed first and the pointer package last, as NuGet
  requires; the previous alphabetical order pushed the pointer first.
- `signing-tool/` added to `.gitignore` (used by the release job's workspace).

## Docs

`README.md` and the packaged `src/AzureSignTool.Cli/readme.md` (installation + requirements rows),
`docs/WALKTHROUGH.md` (consumer CI snippet: .NET 10+ is enough to install),
`.ai/project/tech-stack.md`.

## Caveats

- Consumers still need a .NET 10+ **CLI** to install/update (RID tool packages are an SDK 10
  feature); no runtime is needed afterwards.
- `osx-*` RIDs are not packaged; signing is Windows-only, and those users get a clear install
  error. Adding `any` to the RID list would provide a framework-dependent fallback.
- Key Vault signing of four packages per release adds time to the release job.
