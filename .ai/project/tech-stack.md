# Technology Stack

**Purpose:** This file defines WHAT technologies you use - concrete versions, frameworks, and libraries.

## Runtime & Language

- **.NET SDK:** 10.0.102 (`global.json`, `rollForward: latestFeature`, prerelease allowed)
- **Target Framework:** `net10.0`
- **Language:** C# 14 (`LangVersion` in `Directory.Build.props`)
- **Nullable Reference Types:** Enabled
- **Implicit Usings:** Enabled
- **Unsafe code:** Allowed in `AzureSignTool.Core` (interop structures)

## Application Type

- **`AzureSignTool.Cli`** — console application packed as a .NET global tool
  (`PackAsTool=true`, `ToolCommandName=AzureSignTool`, `AssemblyName=AzureSignTool`).
- **`AzureSignTool.Core`** — signing library, `IsPackable=false`; ships only inside the tool.

## Frameworks & Libraries

| Category | Technology | Version | Notes |
|----------|-----------|---------|-------|
| **CLI parsing** | System.CommandLine | 2.0.14 | commands, options, help, `--version` |
| **Azure authentication** | Azure.Identity | 1.21.0 | `DefaultAzureCredential`, `WorkloadIdentityCredential`, `ManagedIdentityCredential`, `ClientSecretCredential`, … |
| **Key Vault** | Azure.Security.KeyVault.Certificates / Azure.Security.KeyVault.Keys | 4.9.2 / 4.10.2 | certificate lookup and key operations |
| **Globbing** | Microsoft.Extensions.FileSystemGlobbing | 10.0.14 | file arguments and NuGet content filters |
| **Logging** | Microsoft.Extensions.Logging (+ Console) | 10.0.14 | console output, `--verbose` |
| **NuGet packages** | NuGet.Packaging | 7.9.0 | reading and re-signing `.nupkg` files |
| **CMS / signatures** | System.Security.Cryptography.Pkcs | 10.0.14 | package signature blobs |
| **Package IO** | System.IO.Packaging | 10.0.2 | OPC package handling |
| **Native interop** | `mssign32` / `crypt32` via `LibraryImport` | — | Authenticode signing and certificate stores (Windows only) |
| **Testing** | MSTest.TestFramework + MSTest.TestAdapter | 4.5.1 | with Microsoft.NET.Test.Sdk 18.11.0 |
| **Coverage** | coverlet.collector | 10.1.0 | `XPlat Code Coverage`, opencover format |

There is no database, web API, UI or identity-server layer — the tool is a single-purpose CLI.

## Platform Support

- **Runtime OS:** Windows 10 or later. The command tree (help, `--version`, argument validation)
  works on any OS, but `sign` refuses to execute elsewhere and the interop calls are Windows-only.
- **Build/test OS:** Any. The tests that exercise the native signing path
  (`CodeSigningServiceTests`, `MemoryCertificateStoreTests`) are marked
  `[OSCondition(OperatingSystems.Windows)]` and skip on Linux/macOS.

## Cloud Services

- **Cloud Provider:** Microsoft Azure
- **Key Vault:** Azure Key Vault (certificates and keys; `Sign` on key, `Get` on secret/certificate)
- **Hosting / Database / Storage:** Not applicable

## Package Management

- **Version Control:** Central Package Management (`Directory.Packages.props`,
  `ManagePackageVersionsCentrally=true`)
- **Package Source:** nuget.org only (`nuget.config` clears inherited sources)
- **Version Strategy:** Pinned exact versions; `NuGetAuditMode=direct`
- **Publishing:** nuget.org via NuGet trusted publishing (OIDC), `--skip-duplicate`

## Development Tools

- **IDE:** Visual Studio / Rider / VS Code (`.editorconfig`, `.vscode/`)
- **Version Control:** Git, hosted on GitHub (`I-Synergy/AzureSignTool.Cli`)
- **CI/CD:** GitHub Actions — `.github/workflows/ci.yml`; build/test/pack on `ubuntu-latest`,
  signing and release on `windows-2025`
- **Container Runtime:** Not used
- **Local Development:** `dotnet` CLI

## Forbidden Technologies

| ❌ DO NOT USE | ✅ USE INSTEAD | Reason |
|---------------|---------------|--------|
| xUnit / NUnit | MSTest | Project standard |
| Repository/unit-of-work layers | Direct service abstractions | No data layer to abstract |

---

**Remember:** This file documents your actual technology stack. Update it when technologies change. For architectural patterns, see [architecture.md](architecture.md). For workflow preferences, see [preferences.md](preferences.md).
