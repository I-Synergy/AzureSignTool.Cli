# README Verification

**Status:** DONE
Started: 2026-10-27
Completed: 2026-10-27

## Scope

Verified `README.md` (repository landing page, includes the generated standards block) and its
packaged twin `src/AzureSignTool.Cli/readme.md` for correct links, names and commands — then fixed
what the check found.

## Checks run

- Extracted every markdown link; resolved relative paths and GitHub heading anchors (`slug()` rule
  keeps a leading hyphen for emoji headings).
- Parsed every fenced `AzureSignTool sign` example and executed it against the Release binary; each
  one must reach the intended "file does not exist" outcome and must not emit a validation error.
- Compared documented option names and short aliases against `AzureSignTool sign --help`.
- Verified the install command, package id, tool command name, exit codes and requirements against
  `AzureSignTool.Cli.csproj`, `HRESULT.cs` and `global.json`.

## Fixes

- **CI link** pointed at `../.github/workflows/ci.yml` (valid only from `.github/`); now
  `.github/workflows/ci.yml`. The packaged readme uses the absolute GitHub URL, since the package
  ships no `.github/` directory.
- **License** now links to `LICENSE` (absolute URL in the packaged readme).
- **TOC** gained `Standards Compatibility`, which had no entry.
- **CI heading** dropped the variation selector from `🛠️` so its anchor is the same
  `#-building--continuous-integration` shape as every other emoji heading.
- **Requirements** split runtime OS (Windows 10+, native interop) from build/test OS (any) and
  pinned the SDK to `10.0.102` from `global.json`.
- **Exit codes** gained `0x80070057` (E_INVALIDARG) and `0x80131539` (E_PLATFORMNOTSUPPORTED).
- **Installation** states that the package `AzureSignTool.Cli` installs the `AzureSignTool` command.
- **Local build** shows the PowerShell form of `MSBUILDDISABLENODEREUSE` /
  `DOTNET_CLI_USE_MSBUILD_SERVER`.
- **`--colors`** short column corrected from `➖` to `—` (option has no alias).
- **Credential examples** now use `-act ClientSecretCredential` explicitly, and the page documents
  which parameters each credential type consumes.

## Product bug found and fixed

Executing the documented client-secret example exposed a contradiction in `ValidateArguments`:

- `OneTrue(accesstoken, clientId, managedIdentity, credentialType)` rejected `-act` together with
  `-kvi`, while the type-specific validation *requires* `-kvi`/`-kvt`/`-kvs` for
  `ClientSecretCredential` and `-kva` for `AccessTokenCredential` — both types were unusable.
- `-kvi` without `-kvs` was rejected even for `InteractiveBrowserCredential`, which takes a client
  id on its own.

Fix in `src/AzureSignTool.Cli/Commands/SignCommand.cs`:

- The legacy selectors are only required to be exactly one when no `-act` value is supplied.
- With `-act`, `-kvt`/`-kvs` and the parameters the chosen type consumes are allowed;
  `-kvm` and parameters of a different mechanism still conflict.
- The client-id `→` client-secret/tenant rules apply to the service-principal flow only.

Tests added in `CredentialTypeValidationTests.cs`: `ClientSecretCredential` with its parameters,
`InteractiveBrowserCredential` with a client id, and `AccessTokenCredential` with a token all pass
validation. The existing `WorkloadIdentityCredential` + client id conflict test still errors.

## Verification

- `dotnet build -c Release` — 0 errors.
- `dotnet test -c Release` — 52 CLI + 87 Core passed.
- All 10 documented `AzureSignTool sign` examples execute to the expected missing-file outcome.
- Link/anchor/table validation clean for both readmes; `git diff --check` clean.
