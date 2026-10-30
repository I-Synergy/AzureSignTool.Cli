<#
.SYNOPSIS
    Creates the GitHub variables and secrets that .github/workflows/ci.yml expects.

.DESCRIPTION
    Requires the GitHub CLI (https://cli.github.com) installed and authenticated with
    admin rights on the repository:  gh auth login

    Variables are created at repository level so both the build and the release job can
    read them through the `vars` context. Missing values are prompted for; blank answers
    skip that entry, so you can add them later.

    NUGET_USER is created as a repository secret because trusted publishing exchanges the
    GitHub OIDC token for a short-lived nuget.org API key.

    Creates the `production` environment. It receives `-Reviewers` as required reviewers.
    Self-review is allowed because a solo maintainer also triggers the runs. Pull requests do
    not publish, so no separate ungated environment is needed.

.EXAMPLE
    ./scripts/Set-GitHubSettings.ps1

.EXAMPLE
    ./scripts/Set-GitHubSettings.ps1 -AzureKeyVaultUri 'https://my-vault.vault.azure.net/' -AzureSigningCertificate 'code-signing'

.EXAMPLE
    ./scripts/Set-GitHubSettings.ps1 -Reviewers 'ismailhassani','another-maintainer'
#>
param(
    [string]$Repository = 'I-Synergy/AzureSignTool.Cli',

    [string[]]$Environments = @('production'),

    [string]$BuildConfiguration = 'Release',

    [string]$AzureClientId,

    [string]$AzureTenantId,

    [string]$AzureKeyVaultUri,

    [string]$AzureSigningCertificate,

    [string]$TimestampUrl,

    [string]$NuGetUser = 'ismailhassani',

    [string[]]$Reviewers = @('ismailhassani'),

    [string[]]$GatedEnvironments = @('production'),

    [switch]$SkipSecrets
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw 'GitHub CLI (gh) is not installed. See https://cli.github.com, then run: gh auth login'
}

gh auth status --hostname github.com *> $null
if ($LASTEXITCODE -ne 0) {
    throw 'GitHub CLI is not authenticated. Run: gh auth login'
}

function Show-ReadValue {
    param([string]$Prompt, [string]$Current)

    if (-not [string]::IsNullOrWhiteSpace($Current)) {
        return $Current
    }

    $answer = Read-Host $Prompt
    if ([string]::IsNullOrWhiteSpace($answer)) {
        Write-Host "  skipped"
        return $null
    }

    return $answer.Trim()
}

function Set-RepoVariable {
    param([string]$Name, [string]$Value)

    gh variable set $Name --repo $Repository --body $Value
    if ($LASTEXITCODE -ne 0) { throw "Failed to set variable $Name" }
    Write-Output "  variable $Name set"
}

Write-Host "Repository: $Repository"
Write-Host "Environments: $($Environments -join ', ') (approval gates for publishing)"

function Set-Environment {
    param([string]$Name)

    $gated = $GatedEnvironments -contains $Name
    $reviewerIds = @()

    if ($gated -and $Reviewers) {
        $reviewerIds = @(foreach ($login in $Reviewers) {
            $id = gh api "users/$login" --jq '.id'
            if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($id)) {
                throw "Cannot resolve GitHub user '$login' for the '$Name' environment reviewers"
            }
            @{ type = 'User'; id = [int]$id }
        })
    }

    # An explicit empty reviewers array is what removes a gate that was set before, so the body is
    # always sent rather than omitted.
    $body = @{
        reviewers           = @($reviewerIds)
        prevent_self_review = $false
    } | ConvertTo-Json -Depth 5

    $body | gh api --method PUT "repos/$Repository/environments/$Name" --input - *> $null
    if ($LASTEXITCODE -ne 0) { throw "Failed to configure the '$Name' environment" }

    if ($reviewerIds.Count -gt 0) {
        Write-Output "  environment $Name created with required reviewers: $($Reviewers -join ', ')"
    }
    else {
        Write-Output "  environment $Name created without a gate (CI publishes directly)"
    }
}

foreach ($environment in $Environments) {
    Set-Environment -Name $environment
}

Write-Host ''
Write-Host 'Variables (press Enter to skip any that do not apply):'

$variables = [ordered]@{
    BUILD_CONFIGURATION       = Show-ReadValue 'Build configuration [Release]' $BuildConfiguration
    AZURE_CLIENT_ID           = Show-ReadValue 'Azure client id (OIDC)' $AzureClientId
    AZURE_TENANT_ID           = Show-ReadValue 'Azure tenant id (OIDC)' $AzureTenantId
    AZURE_KEYVAULT_URI        = Show-ReadValue 'Key Vault URI (blank = skip package signing)' $AzureKeyVaultUri
    AZURE_SIGNING_CERTIFICATE = Show-ReadValue 'Key Vault certificate name' $AzureSigningCertificate
    TIMESTAMP_URL             = Show-ReadValue 'RFC 3161 timestamp URL' $TimestampUrl
}

foreach ($entry in $variables.GetEnumerator()) {
    if (-not [string]::IsNullOrWhiteSpace($entry.Value)) {
        Set-RepoVariable -Name $entry.Key -Value $entry.Value
    }
}

if (-not $SkipSecrets) {
    Write-Host ''
    Write-Host 'Secrets:'

    $user = Show-ReadValue 'nuget.org username for trusted publishing (NUGET_USER) [ismailhassani]' $NuGetUser
    if ([string]::IsNullOrWhiteSpace($user)) {
        Write-Host '  NUGET_USER skipped - the release job cannot log in to nuget.org until it is set'
    }
    else {
        gh secret set NUGET_USER --repo $Repository --body $user
        if ($LASTEXITCODE -ne 0) { throw 'Failed to set secret NUGET_USER' }
        Write-Host '  secret NUGET_USER set'
    }
}

Write-Host ''
Write-Host 'Current state:'
gh variable list --repo $Repository
if (-not $SkipSecrets) {
    gh secret list --repo $Repository
}
gh api "repos/$Repository/environments" --jq '.environments[].name' 
