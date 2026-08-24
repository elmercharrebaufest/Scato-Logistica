param(
    [string]$Organization,
    [string]$Project,
    [switch]$Login
)

$ErrorActionPreference = 'Stop'

function Invoke-Az {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = & az @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw ("az {0} failed: {1}" -f ($Arguments -join ' '), ($output | Out-String).Trim())
    }

    return $output
}

function Get-RepoDefaults {
    $defaultsPath = Join-Path $PSScriptRoot 'azure-devops.defaults.psd1'
    if (Test-Path $defaultsPath) {
        return Import-PowerShellDataFile -Path $defaultsPath
    }

    return @{
        Organization = 'https://dev.azure.com/molinosagro'
        Project      = 'Scato Logistica'
    }
}

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI no esta instalado. Instala Azure CLI antes de ejecutar este script.'
}

$repoDefaults = Get-RepoDefaults
if (-not $Organization) {
    $Organization = $repoDefaults.Organization
}

if (-not $Project) {
    $Project = $repoDefaults.Project
}

try {
    Invoke-Az -Arguments @('extension', 'show', '--name', 'azure-devops', '--query', 'name', '-o', 'tsv') | Out-Null
} catch {
    Write-Host 'Instalando extension azure-devops...' -ForegroundColor Yellow
    Invoke-Az -Arguments @('extension', 'add', '--name', 'azure-devops') | Out-Null
}

if ($Login) {
    try {
        Invoke-Az -Arguments @('account', 'show', '--query', 'user.name', '-o', 'tsv') | Out-Null
    } catch {
        Write-Host 'Iniciando sesion en Azure CLI...' -ForegroundColor Yellow
        Invoke-Az -Arguments @('login', '--use-device-code') | Out-Null
    }
}

Write-Host 'Configurando defaults de Azure DevOps...' -ForegroundColor Cyan
Invoke-Az -Arguments @(
    'devops', 'configure',
    '--defaults',
    ('organization={0}' -f $Organization),
    ('project={0}' -f $Project)
) | Out-Null

try {
    Invoke-Az -Arguments @('devops', 'configure', '--list') | Out-Null
} catch {
    Write-Host 'Defaults configurados; inicia sesion para ver el listado completo.' -ForegroundColor Yellow
}

Write-Host ''
Write-Host 'Azure DevOps CLI listo.' -ForegroundColor Green
Write-Host ('Organization: {0}' -f $Organization) -ForegroundColor Gray
Write-Host ('Project:      {0}' -f $Project) -ForegroundColor Gray
Write-Host ''
Write-Host 'Verificacion:' -ForegroundColor Cyan
Write-Host '  az devops project show' -ForegroundColor Gray
Write-Host '  az repos list' -ForegroundColor Gray
Write-Host '  az boards work-item show --id 1234' -ForegroundColor Gray
