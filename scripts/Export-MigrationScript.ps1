# Exports an idempotent EF Core migration SQL script for a service into
#   services/<S>/<S>.Infrastructure/Migrations/Scripts/<yyyyMMddHHmmss>-<S>-full.sql
# (or <ts>-<S>-<From>-to-<To>.sql for a delta) and opens it in the editor.
#
# Usage (from backend/, or anywhere — the script resolves backend/ itself):
#   ./scripts/Export-MigrationScript.ps1 -Service Ordering
#   ./scripts/Export-MigrationScript.ps1 -Service Ordering -From 20260923155704_InitialCreate -To 20260924090000_AddSomething
#   ./scripts/Export-MigrationScript.ps1 -Service Ordering -NoOpen   # skip auto-open
param(
    [Parameter(Mandatory = $true)][string]$Service,
    [string]$From = "",
    [string]$To = "",
    [switch]$NoOpen
)

$ErrorActionPreference = "Stop"
$backend = Split-Path -Parent $PSScriptRoot
Set-Location -LiteralPath $backend

$ts = Get-Date -Format "yyyyMMddHHmmss"
$dir = "services/$Service/$Service.Infrastructure/Migrations/Scripts"
New-Item -ItemType Directory -Path $dir -Force | Out-Null

if ([string]::IsNullOrWhiteSpace($From) -and [string]::IsNullOrWhiteSpace($To)) {
    $name = "$ts-$Service-full.sql"
    $efArgs = @("migrations", "script", "--idempotent")
}
else {
    $name = "$ts-$Service-$From-to-$To.sql"
    $efArgs = @("migrations", "script", $From, $To, "--idempotent")
}
$out = "$dir/$name"

dotnet build "services/$Service/$Service.API/$Service.API.csproj" /p:DisableImplicitNuGetFallbackFolder=true
if ($LASTEXITCODE -ne 0) { throw "Build failed for $Service.API" }

$efArgs += @("--project", "services/$Service/$Service.Infrastructure/$Service.Infrastructure.csproj",
    "--startup-project", "services/$Service/$Service.API/$Service.API.csproj",
    "--no-build", "--output", $out)
dotnet ef @efArgs

Write-Output "Script written: $out"
if (-not $NoOpen) { Invoke-Item -LiteralPath $out }
