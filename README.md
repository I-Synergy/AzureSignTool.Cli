# 🔐 AzureSignTool

[![CI](https://github.com/I-Synergy/AzureSignTool.Cli/actions/workflows/ci.yml/badge.svg)](https://github.com/I-Synergy/AzureSignTool.Cli/actions/workflows/ci.yml)

A code signing tool that uses **Azure Key Vault** for performing the signing process. Similar to `signtool` in the Windows SDK, but leverages Azure Key Vault certificates for secure, centralized key management.

> 📦 Based on [AzureSignTool](https://github.com/vcsjones/AzureSignTool) by Kevin Jones.

---

## 📑 Table of Contents

- [⚡ Quick Start](#-quick-start)
- [📥 Installation](#-installation)
- [🔑 Authentication Methods](#-authentication-methods)
- [💻 Usage Examples](#-usage-examples)
- [📖 Command Reference](#-command-reference)
- [📁 Supported Formats](#-supported-formats)
- [🚦 Exit Codes](#-exit-codes)
- [🔧 Troubleshooting](#-troubleshooting)
- [🛠 Building & Continuous Integration](#-building--continuous-integration)
- [📋 Requirements](#-requirements)
- [📄 License](#-license)
- [🤖 AI-Assisted Development](#-ai-assisted-development)
- [✅ Standards Compatibility](#standards-compatibility)

---

## ⚡ Quick Start

```powershell
# Install the tool
dotnet tool install --global AzureSignTool.Cli

# Sign a file using Azure CLI credentials (for local development)
AzureSignTool sign `
    -kvu https://my-vault.vault.azure.net `
    -kvc my-certificate-name `
    -act AzureCliCredential `
    -tr http://timestamp.digicert.com `
    myfile.exe
```

---

## 📥 Installation

Install as a global .NET tool. The package is `AzureSignTool.Cli`; it installs the
`AzureSignTool` command:

```powershell
dotnet tool install --global AzureSignTool.Cli
# then, from any shell:
AzureSignTool --version
```

---

## 🔑 Authentication Methods

This tool supports multiple Azure authentication methods. For **production scenarios**, use the `--azure-credential-type` (`-act`) parameter to explicitly specify the credential type rather than relying on `DefaultAzureCredential`.

### 🎯 Choosing the Right Credential Type

| Scenario | Credential Type | When to Use |
|:---------|:----------------|:------------|
| ☸️ Kubernetes / Containers | `WorkloadIdentityCredential` | CI/CD pipelines in AKS with workload identity federation |
| ☁️ Azure VMs / App Services | `ManagedIdentityCredential` | Applications running on Azure compute resources |
| 🔄 CI/CD with Service Principal | `ClientSecretCredential` | Automated builds with explicit client credentials |
| 🖥️ Local Development | `AzureCliCredential` | Developers authenticated via `az login` |
| 🌐 Interactive Use | `InteractiveBrowserCredential` | Manual signing operations requiring user login |
| 🔧 Environment Variables | `EnvironmentCredential` | Applications configured via Azure environment variables |

### ⚠️ Why Avoid `DefaultAzureCredential` in Production?

According to [Microsoft's documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential), `DefaultAzureCredential` is designed for development convenience but is **not recommended for production** because it:

- ❌ Tries multiple credential types in sequence, causing unpredictable behavior
- ❌ May attempt unnecessary authentication methods, impacting performance
- ❌ Makes troubleshooting more difficult due to its "magic" discovery chain

Using `--azure-credential-type` provides:

- ✅ **Explicit control** over which authentication method is used
- ✅ **Better security** by limiting credential discovery
- ✅ **Improved performance** by avoiding unnecessary auth attempts
- ✅ **Easier troubleshooting** with predictable authentication flow

> ℹ️ **Combining parameters:** a credential type consumes the parameters it needs, so `-kvi`,
> `-kvt` and `-kvs` are valid alongside `ClientSecretCredential`, `-kvi`/`-kvt` alongside
> `InteractiveBrowserCredential`, and `-kva` alongside `AccessTokenCredential`. The legacy
> selectors `--azure-key-vault-accesstoken`, `--azure-key-vault-client-id` and
> `--azure-key-vault-managed-identity` cannot be combined with a type that does not consume them.

---

## 💻 Usage Examples

### 🖥️ Using Azure CLI Credential (Local Development)

```powershell
AzureSignTool sign -du "https://example.com" `
    -fd sha384 `
    -kvu https://my-vault.vault.azure.net `
    -act AzureCliCredential `
    -kvc my-certificate-name `
    -tr http://timestamp.digicert.com `
    -td sha384 `
    -v `
    myfile.exe
```

### ☸️ Using Workload Identity (Kubernetes / Containers)

```powershell
AzureSignTool sign -du "https://example.com" `
    -fd sha384 `
    -kvu https://my-vault.vault.azure.net `
    -act WorkloadIdentityCredential `
    -kvc my-certificate-name `
    -tr http://timestamp.digicert.com `
    -td sha384 `
    -v `
    myfile.exe
```

### ☁️ Using Managed Identity (Azure VMs / App Services)

```powershell
AzureSignTool sign -du "https://example.com" `
    -fd sha384 `
    -kvu https://my-vault.vault.azure.net `
    -act ManagedIdentityCredential `
    -kvc my-certificate-name `
    -tr http://timestamp.digicert.com `
    -td sha384 `
    -v `
    myfile.exe
```

### 🔄 Using Client Secret (CI/CD Pipelines)

```powershell
AzureSignTool sign -du "https://example.com" `
    -fd sha384 `
    -kvu https://my-vault.vault.azure.net `
    -act ClientSecretCredential `
    -kvi 01234567-abcd-ef012-0000-0123456789ab `
    -kvt 01234567-abcd-ef012-0000-0123456789ab `
    -kvs <client-secret> `
    -kvc my-certificate-name `
    -tr http://timestamp.digicert.com `
    -td sha384 `
    -v `
    myfile.exe
```

### 🌐 Using Interactive Browser

```powershell
AzureSignTool sign -du "https://example.com" `
    -fd sha384 `
    -kvu https://my-vault.vault.azure.net `
    -act InteractiveBrowserCredential `
    -kvi 01234567-abcd-ef012-0000-0123456789ab `
    -kvt 01234567-abcd-ef012-0000-0123456789ab `
    -kvc my-certificate-name `
    -tr http://timestamp.digicert.com `
    -td sha384 `
    -v `
    myfile.exe
```

> 💡 **Tip:** Use `--help` or `sign --help` for detailed parameter information.

---

## 📖 Command Reference

### 🔐 Azure Key Vault Authentication

| Parameter | Short | Required | Description |
|:----------|:-----:|:--------:|:------------|
| `--azure-key-vault-url` | `-kvu` | ✅ | Fully qualified URL of the key vault (e.g., `https://my-vault.vault.azure.net`) |
| `--azure-key-vault-certificate` | `-kvc` | ✅ | Name of the certificate for signing |
| `--azure-key-vault-certificate-version` | `-kvcv` | ➖ | Specific certificate version (defaults to latest) |
| `--azure-key-vault-client-id` | `-kvi` | ⚙️ | Client ID for service principal authentication |
| `--azure-key-vault-client-secret` | `-kvs` | ⚙️ | Client secret for service principal authentication |
| `--azure-key-vault-tenant-id` | `-kvt` | ⚙️ | Tenant ID for service principal authentication |
| `--azure-key-vault-accesstoken` | `-kva` | ⚙️ | Pre-acquired access token for authentication |
| `--azure-key-vault-managed-identity` | `-kvm` | ⚙️ | Use DefaultAzureCredential (not recommended for production) |
| `--azure-credential-type` | `-act` | ⚙️ | **Recommended.** Explicit credential type (see [Authentication Methods](#-authentication-methods)) |
| `--azure-authority` | `-au` | ➖ | Azure Authority URL (for sovereign clouds) |

> ✅ = Required | ➖ = Optional | ⚙️ = Required depending on authentication method

#### 🎛️ Available Credential Types (`-act`)

| Value | Description |
|:------|:------------|
| `DefaultAzureCredential` | 🔄 Tries multiple credential types in sequence (not recommended for production) |
| `ManagedIdentityCredential` | ☁️ Azure Managed Identity |
| `WorkloadIdentityCredential` | ☸️ Workload identity federation (Kubernetes/containers) |
| `ClientSecretCredential` | 🔑 Service principal with client secret |
| `InteractiveBrowserCredential` | 🌐 Interactive browser authentication |
| `EnvironmentCredential` | 🔧 Authenticate using environment variables |
| `AzureCliCredential` | 🖥️ Authenticate using Azure CLI |
| `AzurePowerShellCredential` | 💠 Authenticate using Azure PowerShell |
| `AccessTokenCredential` | 🎫 Use a pre-acquired access token |

### ✍️ Signing Options

| Parameter | Short | Default | Description |
|:----------|:-----:|:-------:|:------------|
| `--description` | `-d` | ➖ | Description of the signed content (same as `/d` in signtool) |
| `--description-url` | `-du` | ➖ | URL with more information about the content (same as `/du` in signtool) |
| `--file-digest` | `-fd` | sha256 | Hash algorithm for file digest: `sha1`, `sha256`, `sha384`, `sha512` |
| `--timestamp-rfc3161` | `-tr` | ➖ | RFC3161 timestamp server URL (recommended) |
| `--timestamp-authenticode` | `-t` | ➖ | Legacy Authenticode timestamp server (deprecated) |
| `--timestamp-digest` | `-td` | sha256 | Hash algorithm for timestamp: `sha1`, `sha256`, `sha384`, `sha512` |
| `--additional-certificates` | `-ac` | ➖ | Additional certificates for the chain (can be specified multiple times) |

### ⚙️ Processing Options

| Parameter | Short | Default | Description |
|:----------|:-----:|:-------:|:------------|
| `--continue-on-error` | `-coe` | ❌ | Continue signing remaining files if one fails |
| `--input-file-list` | `-ifl` | ➖ | Path to text file containing files to sign (one per line) |
| `--skip-signed` | `-s` | ❌ | Skip files that are already signed |
| `--append-signature` | `-as` | ❌ | Append signature instead of replacing (Windows 11+) |
| `--max-degree-of-parallelism` | `-mdop` | 4 | Maximum concurrent signing operations |
| `--sign-nuget-contents` | `-snc` | ❌ | Sign binaries inside NuGet packages |
| `--sign-nuget-contents-filter` | `-sncf` | ➖ | Glob pattern to filter which binaries to sign inside NuGet packages |

### 📤 Output Options

| Parameter | Short | Description |
|:----------|:-----:|:------------|
| `--verbose` | `-v` | 📝 Show detailed output including authentication info |
| `--quiet` | `-q` | 🔇 Suppress all output |
| `--colors` | — | 🎨 Enable colored output |

### 🔬 Advanced Options

| Parameter | Short | Description |
|:----------|:-----:|:------------|
| `--page-hashing` | `-ph` | Generate page hashes for executable files |
| `--no-page-hashing` | `-nph` | Suppress page hashes for executable files |

---

## 📁 Supported Formats

This tool supports the same formats as the Windows SDK `signtool`:

| Format | Extensions |
|:-------|:-----------|
| 🖥️ Portable Executable | `.exe`, `.dll` |
| 📦 Cabinet Files | `.cab` |
| 💿 Windows Installer | `.msi`, `.msix` |
| 📱 APPX/MSIX Packages | `.appx`, `.msix`, `.appxbundle`, `.msixbundle` |
| 📦 NuGet Packages | `.nupkg`, `.snupkg` |

### 📦 NuGet Package Signing

When signing NuGet packages, the tool supports **two types of signing**:

#### 1️⃣ Package-Level Signature (Default)

Creates a NuGet author signature (`.signature.p7s`) for the package itself.

```powershell
AzureSignTool sign `
    -kvu https://my-vault.vault.azure.net `
    -act ClientSecretCredential `
    -kvi <client-id> -kvt <tenant-id> -kvs <client-secret> `
    -kvc my-certificate `
    -tr http://timestamp.digicert.com `
    MyPackage.1.0.0.nupkg
```

#### 2️⃣ Package + Content Signing (Optional)

Signs both the package and all binaries inside it (`.dll`, `.exe`, `.winmd`).

```powershell
AzureSignTool sign `
    -kvu https://my-vault.vault.azure.net `
    -act ClientSecretCredential `
    -kvi <client-id> -kvt <tenant-id> -kvs <client-secret> `
    -kvc my-certificate `
    -tr http://timestamp.digicert.com `
    --sign-nuget-contents `
    MyPackage.1.0.0.nupkg
```

#### 3️⃣ Filtered Content Signing

Sign only specific binaries inside the package using a glob pattern (useful when packages contain third-party DLLs that are already signed).

```powershell
AzureSignTool sign `
    -kvu https://my-vault.vault.azure.net `
    -act ClientSecretCredential `
    -kvi <client-id> -kvt <tenant-id> -kvs <client-secret> `
    -kvc my-certificate `
    -tr http://timestamp.digicert.com `
    --sign-nuget-contents `
    --sign-nuget-contents-filter "ISynergy.Framework.*" `
    MyPackage.1.0.0.nupkg
```

> ℹ️ **Note:** Symbol packages (`.snupkg`) do not support package-level signatures per NuGet specifications. The certificate must have the Code Signing enhanced key usage (EKU).

---

## 🚦 Exit Codes

| Code | Name | Description |
|:----:|:-----|:------------|
| `0` | ✅ S_OK | All files signed successfully |
| `0x20000001` | ⚠️ Partial Success | Some files signed, some failed |
| `0xA0000002` | ❌ All Failed | All files failed to sign |
| `0x80070057` | ❌ E_INVALIDARG | Argument validation failed (nothing was signed) |
| `0x80131539` | ❌ E_PLATFORMNOTSUPPORTED | `sign` was run on a non-Windows OS or older Windows |

---

## 🔧 Troubleshooting

### 📝 Enable Verbose Logging

Use the `--verbose` (`-v`) flag to see detailed authentication and signing information:

```powershell
AzureSignTool sign `
    -kvu https://my-vault.vault.azure.net `
    -act WorkloadIdentityCredential `
    -kvc my-certificate `
    -v `
    myfile.exe
```

The verbose output includes:
- 🔑 Which credential type is being used
- ⚙️ Configuration parameters (Tenant ID, Client ID, etc.)
- 🔧 Environment variables being checked
- ⚠️ Warnings if using `DefaultAzureCredential` in production

### 🔍 Common Authentication Issues

#### ☸️ WorkloadIdentityCredential (Kubernetes)

Ensure these environment variables are set:

| Variable | Description |
|:---------|:------------|
| `AZURE_TENANT_ID` | 🏢 Azure AD tenant ID |
| `AZURE_CLIENT_ID` | 🆔 Application (client) ID |
| `AZURE_FEDERATED_TOKEN_FILE` | 📄 Path to the projected service account token |

#### ☁️ ManagedIdentityCredential (Azure VMs / App Services)

- ✅ Verify the Azure resource has a managed identity assigned
- ✅ Ensure the identity has appropriate Key Vault access policies or RBAC permissions

#### 🖥️ AzureCliCredential

```powershell
# Ensure you're logged in
az login

# Verify your account
az account show
```

#### 💠 AzurePowerShellCredential

```powershell
# Ensure you're logged in
Connect-AzAccount

# Verify your context
Get-AzContext
```

---

## 🛠 Building & Continuous Integration

### 💻 Local build

```powershell
dotnet restore AzureSignTool.Cli.slnx
dotnet build AzureSignTool.Cli.slnx -c Release -p:GeneratePackageOnBuild=false -nodeReuse:false
dotnet test AzureSignTool.Cli.slnx -c Release --no-build
dotnet pack AzureSignTool.Cli.slnx -c Release --no-build -o packages
```

Set `MSBUILDDISABLENODEREUSE=1` (and pass `-nodeReuse:false`) when building locally — MSBuild
node reuse is what exhausts memory on developer machines (see `.ai/project/preferences.md`):

```powershell
$env:MSBUILDDISABLENODEREUSE = "1"
$env:DOTNET_CLI_USE_MSBUILD_SERVER = "0"
```

### 🔄 Pipeline

CI/CD runs on **GitHub Actions**: [`.github/workflows/ci.yml`](.github/workflows/ci.yml).
It replaced the former Azure DevOps pipeline.

| Job | Runner | What it does |
|:----|:-------|:-------------|
| `build` | `ubuntu-latest` | restore → build → pack → verify the package → test with opencover coverage → upload `nupkgs` and `test-results` |
| `release` | `windows-2025` | sign the packages with the `AzureSignTool` built by the `build` job, through Azure Key Vault (OIDC) — the NuGet signature plus an Authenticode signature on our own assemblies inside the package (`--sign-nuget-contents-filter "AzureSignTool*"`) — push to nuget.org (trusted publishing). A push to `main` publishes a release through the `production` environment and creates the `<version>` tag + GitHub release; a pull request from this repository publishes a `-preview` package through the `acceptance` environment and creates no tag |

Build and tests are platform-neutral; only the native signing path is Windows-bound
(`mssign32`/`crypt32`). The two test classes that exercise it —
`CodeSigningServiceTests` and `MemoryCertificateStoreTests` — carry
`[OSCondition(OperatingSystems.Windows)]` and are skipped on Linux. The `release` job stays on
Windows because the tool itself refuses to run elsewhere.

Versions come from the workflow clock in UTC as `yyyy.1MMdd.1HHmm` (`2026.11027.11034`); pull
requests get a `-preview` package suffix (`2026.11027.11034-preview`). No file is bumped for a
release: the tag and GitHub release use the build number directly (no `v` prefix). Fork PRs never
publish — they get no secrets — so only branches in this repository can produce a preview.

### 🔐 Repository configuration

Nothing here is needed for the build and test job — only for publishing. One command creates
everything, using an authenticated `gh` CLI:

```powershell
./scripts/Set-GitHubSettings.ps1
```

- **Environments** (Settings → Environments): `production` gates a release from `main` with
  required reviewers (`-Reviewers`, default `ismailhassani`); `acceptance` is deliberately
  **ungated**, so a pull request publishes its preview from CI without approval.
- **Variables**: `BUILD_CONFIGURATION` (optional, defaults to `Release`), `AZURE_CLIENT_ID`,
  `AZURE_TENANT_ID`, `AZURE_KEYVAULT_URI`, `AZURE_SIGNING_CERTIFICATE`, `TIMESTAMP_URL`. No
  subscription id is needed: the release job requests the OIDC token directly and signs through the
  Key Vault access policy, so the identity needs no Azure role assignment.
- **Secret**: `NUGET_USER` — the nuget.org username used for trusted publishing
  (`ismailhassani`, not the `I-Synergy` organization). The release job fails fast with a clear
  message when it is missing.

nuget.org trusted publishing pins the environment name, so each environment needs its own policy.
Signing is skipped when `AZURE_KEYVAULT_URI` is not configured — the release job logs a warning and
publishes unsigned packages in that case.

---

## 📋 Requirements

| Requirement | Version |
|:------------|:--------|
| 🖥️ Runtime OS | Windows 10 or later (signing calls `mssign32`/`crypt32`) |
| 🛠️ Build/test OS | Any — Windows, Linux or macOS |
| 🧰 .NET SDK (for building) | 10.0.102 (`global.json`, `rollForward: latestFeature`) |

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

This project is based on [AzureSignTool](https://github.com/vcsjones/AzureSignTool) by [Kevin Jones](https://github.com/vcsjones).

## 🤖 AI-Assisted Development

This repository is developed with AI agents. `.ai/` is the single source of truth for patterns,
skills and project context, and every client reads it through directory junctions. Code and design
work is delegated to these subagents:

| Agent | Role |
|-------|------|
| `architect` | Feature design, pattern selection, component boundaries, architecture analysis |
| `reviewer` | Code quality, SOLID, security review, architecture audit, ISO/IEC 25010 findings |
| `security` | Blast-radius and exposure analysis, threat modelling, OWASP review, dependency and secret audits |
| `tester` | MSTest test design, BDD scenarios, integration test strategy |
| `designer` | Visual design — colour palettes, typography, branding, design tokens |
| `developer` | .NET/C# implementation — refactoring, builds |
| `ui-developer` | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `ui-tester` | Playwright E2E tests, accessibility checks, visual regression |
| `writer` | XML docs, READMEs, ADRs, technical prose |

Each agent's `model:` frontmatter in `.ai/agents/` carries a tier alias (`sonnet` or `haiku`); the
concrete model bound to that tier is chosen by the backend runtime outside this repository.

---

<!-- BEGIN standards-status -->
## Standards Compatibility

Engineering artifacts produced by this repository correspond to published standards, and their status
is computed from validator runs rather than asserted. The full view — what is produced, what is
aligned in practice only, and what stays outside a code repository's reach — is in
[`COMPLIANCE.md`](COMPLIANCE.md).

| Compatibility | Meaning | Standards |
|---|---|---|
| **Evidence** | An artifact a certification auditor can sample directly | *(none)* |
| **Aligned** | Engineering practice follows the standard's model; no audit artifact is produced | ISO/IEC/IEEE 29119-3:2021, ISO/IEC 25010:2023 |
| **Organizational** | Certifiable only at organization level; this repository supplies nothing for it | ISO/IEC 27001, ISO/IEC 27701 / ISO/IEC 27017 / ISO/IEC 27018, ISO 9001, ISO/IEC 15504 / ISO/IEC 33001, IEC 62304, ISO/IEC 42001 |

**This is not a certification and it asserts no conformity with any standard.** Certification is
issued by an accredited third-party body after auditing an organizational management system; it
cannot be established by a code repository.

<!-- standards-status: source=.ai/reference/standards.md generated=2026-10-27 revision=a409723 validators=none -->
<!-- END standards-status -->
