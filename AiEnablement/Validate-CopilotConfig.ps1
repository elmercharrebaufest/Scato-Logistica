<#
.SYNOPSIS
    Valida coherencia entre .github/copilot-config.yml y .github/agents/*.agent.md.

.DESCRIPTION
    Compara los modelos declarados en copilot-config.yml (custom_agents.model)
    con los modelos en el frontmatter de cada .agent.md (model:).
    Reporta divergencias, agentes huerfanos y entradas de config sin archivo.

.EXAMPLE
    .\AiEnablement\Validate-CopilotConfig.ps1

.EXAMPLE
    .\AiEnablement\Validate-CopilotConfig.ps1 -Fix
    # Sobrescribe el modelo en copilot-config.yml para sincronizarlo con cada .agent.md
    # (el .agent.md es la fuente de verdad)
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [switch]$Fix   # Actualiza copilot-config.yml para que coincida con los .agent.md
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-RepoRoot {
    $current = $PSScriptRoot
    while ($current) {
        if (Test-Path (Join-Path $current '.github\copilot-config.yml')) {
            return $current
        }

        $parent = Split-Path -Parent $current
        if ($parent -eq $current) { break }
        $current = $parent
    }

    throw 'No se pudo resolver la raiz del repo (.github\copilot-config.yml).'
}

$RepoRoot   = Resolve-RepoRoot
$ConfigFile = Join-Path $RepoRoot '.github\copilot-config.yml'
$AgentsDir  = Join-Path $RepoRoot '.github\agents'

# ─────────────────────────────────────────────────────────────
# Colores y helpers
# ─────────────────────────────────────────────────────────────
function Write-Ok      ($msg) { Write-Host "  ✅  $msg" -ForegroundColor Green  }
function Write-Warn    ($msg) { Write-Host "  ⚠️   $msg" -ForegroundColor Yellow }
function Write-Err     ($msg) { Write-Host "  🔴  $msg" -ForegroundColor Red    }
function Write-Info    ($msg) { Write-Host "  ℹ️   $msg" -ForegroundColor Cyan   }

# ─────────────────────────────────────────────────────────────
# 1. Leer copilot-config.yml
# ─────────────────────────────────────────────────────────────
if (-not (Test-Path $ConfigFile)) {
    Write-Err "No se encontró .github\copilot-config.yml — abortar."
    exit 1
}

$configLines = Get-Content $ConfigFile

# Parsear sección custom_agents: bloque "- name: X \n   model: Y"
$configAgents = [ordered]@{}   # slug -> model

$inCustom  = $false
$curName   = $null

foreach ($line in $configLines) {
    if ($line -match '^\s*custom_agents\s*:') { $inCustom = $true; continue }

    if ($inCustom) {
        # Salir si llegamos a una sección de nivel raíz distinta
        if ($line -match '^[a-zA-Z]') { $inCustom = $false; $curName = $null; continue }

        if ($line -match '^\s+-\s+name:\s*(.+?)\s*$') {
            $curName = $Matches[1].Trim()
        }
        elseif ($curName -and $line -match '^\s+model:\s*(.+?)\s*$') {
            $configAgents[$curName] = $Matches[1].Trim()
            $curName = $null
        }
    }
}

# ─────────────────────────────────────────────────────────────
# 2. Leer .agent.md files
# ─────────────────────────────────────────────────────────────
$agentFiles = Get-ChildItem $AgentsDir -Filter '*.agent.md' | Sort-Object Name

$agentMeta = [ordered]@{}   # slug -> @{ DisplayName, Model, File }

foreach ($file in $agentFiles) {
    $slug    = $file.BaseName -replace '\.agent$', ''
    $content = Get-Content $file.FullName -Raw

    $nameMatch  = [regex]::Match($content, '(?m)^name:\s*["\x27]?(.+?)["\x27]?\s*$')
    $modelMatch = [regex]::Match($content, '(?m)^model:\s*(.+?)\s*$')

    $agentMeta[$slug] = @{
        DisplayName = if ($nameMatch.Success)  { $nameMatch.Groups[1].Value.Trim()  } else { $slug }
        Model       = if ($modelMatch.Success) { $modelMatch.Groups[1].Value.Trim() } else { $null }
        File        = $file.Name
    }
}

# ─────────────────────────────────────────────────────────────
# 3. Comparar y generar reporte
# ─────────────────────────────────────────────────────────────
Write-Host ''
Write-Host '╔══════════════════════════════════════════════════════════════╗' -ForegroundColor Cyan
Write-Host '║     Validate-CopilotConfig — Scato Logistica                ║' -ForegroundColor Cyan
Write-Host '╚══════════════════════════════════════════════════════════════╝' -ForegroundColor Cyan
Write-Host ''

$issues   = [System.Collections.Generic.List[hashtable]]::new()
$synced   = 0
$warnings = 0
$errors   = 0

# 3a. Por cada .agent.md: ¿está en config? ¿modelos coinciden?
Write-Host '── Agentes custom (.agent.md) ──────────────────────────────────' -ForegroundColor DarkGray
foreach ($slug in $agentMeta.Keys) {
    $meta = $agentMeta[$slug]

    if (-not $configAgents.Contains($slug)) {
        Write-Err "$slug → falta entrada en copilot-config.yml custom_agents"
        $issues.Add(@{ Type = 'MissingInConfig'; Slug = $slug; AgentModel = $meta.Model })
        $errors++
        continue
    }

    $cfgModel   = $configAgents[$slug]
    $agentModel = $meta.Model

    if ($null -eq $agentModel) {
        Write-Warn "$slug → .agent.md no declara 'model:' (config: $cfgModel)"
        $warnings++
        continue
    }

    if ($cfgModel -ne $agentModel) {
        Write-Warn "$slug → DIVERGENCIA   config=$cfgModel   agent.md=$agentModel"
        $issues.Add(@{ Type = 'ModelMismatch'; Slug = $slug; ConfigModel = $cfgModel; AgentModel = $agentModel })
        $warnings++
    } else {
        Write-Ok "$slug → $cfgModel"
        $synced++
    }
}

# 3b. Por cada entrada en config: ¿existe el .agent.md?
Write-Host ''
Write-Host '── Entradas en copilot-config.yml sin .agent.md ────────────────' -ForegroundColor DarkGray
$orphanConfig = 0
foreach ($slug in $configAgents.Keys) {
    if (-not $agentMeta.Contains($slug)) {
        Write-Err "$slug → declarado en copilot-config.yml pero sin .github/agents/$slug.agent.md"
        $issues.Add(@{ Type = 'MissingAgentFile'; Slug = $slug; ConfigModel = $configAgents[$slug] })
        $errors++
        $orphanConfig++
    }
}
if ($orphanConfig -eq 0) {
    Write-Info "Todas las entradas de config tienen su .agent.md correspondiente."
}

# ─────────────────────────────────────────────────────────────
# 4. Resumen
# ─────────────────────────────────────────────────────────────
Write-Host ''
Write-Host '── Resumen ─────────────────────────────────────────────────────' -ForegroundColor DarkGray
Write-Host "  Agentes revisados : $($agentMeta.Count)"
Write-Host "  ✅  En sincronía   : $synced"
if ($warnings -gt 0) { Write-Host "  ⚠️   Advertencias  : $warnings" -ForegroundColor Yellow }
if ($errors   -gt 0) { Write-Host "  🔴  Errores        : $errors"   -ForegroundColor Red    }
Write-Host ''

if ($issues.Count -eq 0) {
    Write-Ok "copilot-config.yml y .agent.md están en perfecta sincronía."
    Write-Host ''
    exit 0
}

# ─────────────────────────────────────────────────────────────
# 5. Modo -Fix: sincronizar copilot-config.yml desde .agent.md
# ─────────────────────────────────────────────────────────────
if ($Fix) {
    Write-Host '── Aplicando correcciones (-Fix) ───────────────────────────────' -ForegroundColor DarkGray

    $configContent = Get-Content $ConfigFile -Raw

    foreach ($issue in $issues) {
        if ($issue.Type -eq 'ModelMismatch') {
            $slug       = $issue.Slug
            $oldModel   = $issue.ConfigModel
            $newModel   = $issue.AgentModel

            # Reemplazar "- name: {slug}\n    model: {old}" por "- name: {slug}\n    model: {new}"
            # Patrón: línea con "name: {slug}" seguida de línea con "model: {oldModel}"
            $pattern     = "(?m)(- name:\s*$([regex]::Escape($slug))\r?\n(?:.*\r?\n)*?\s+model:\s*)$([regex]::Escape($oldModel))"
            $replacement = '${1}' + $newModel

            if ($configContent -match $pattern) {
                $configContent = $configContent -replace $pattern, $replacement
                Write-Ok "$slug → actualizado en config: $oldModel → $newModel"
            } else {
                Write-Warn "$slug → no se pudo localizar el bloque en config para reemplazar"
            }
        }
        elseif ($issue.Type -eq 'MissingInConfig') {
            Write-Warn "$($issue.Slug) → falta en config. Agregar manualmente:"
            Write-Host "    - name: $($issue.Slug)" -ForegroundColor Gray
            Write-Host "      model: $($issue.AgentModel)" -ForegroundColor Gray
            Write-Host "      context_tier: default" -ForegroundColor Gray
            Write-Host "      effort: medium" -ForegroundColor Gray
        }
    }

    if ($PSCmdlet.ShouldProcess($ConfigFile, 'Sobrescribir con modelos sincronizados')) {
        [System.IO.File]::WriteAllText($ConfigFile, $configContent, [System.Text.Encoding]::UTF8)
        Write-Ok "copilot-config.yml guardado."
    }
    Write-Host ''
    exit $(if ($errors -gt 0) { 1 } else { 0 })
}

# Sin -Fix: indicar cómo corregir
Write-Host '── Cómo corregir ───────────────────────────────────────────────' -ForegroundColor DarkGray
foreach ($issue in $issues) {
    switch ($issue.Type) {
        'ModelMismatch' {
            Write-Host "  $($issue.Slug): actualizar copilot-config.yml o .agent.md para usar el mismo modelo." -ForegroundColor Yellow
            Write-Host "    config=$($issue.ConfigModel)  vs  agent.md=$($issue.AgentModel)" -ForegroundColor Gray
            Write-Host "    Ejecutar con -Fix para sincronizar automáticamente (usa .agent.md como fuente de verdad)." -ForegroundColor Gray
        }
        'MissingInConfig' {
            Write-Host "  $($issue.Slug): agregar a copilot-config.yml custom_agents:" -ForegroundColor Red
            Write-Host "    - name: $($issue.Slug)" -ForegroundColor Gray
            Write-Host "      model: $($issue.AgentModel)" -ForegroundColor Gray
            Write-Host "      context_tier: default" -ForegroundColor Gray
            Write-Host "      effort: medium" -ForegroundColor Gray
        }
        'MissingAgentFile' {
            Write-Host "  $($issue.Slug): crear .github/agents/$($issue.Slug).agent.md" -ForegroundColor Red
            Write-Host "    o eliminar la entrada de copilot-config.yml custom_agents." -ForegroundColor Gray
        }
    }
}
Write-Host ''

exit $(if ($errors -gt 0) { 1 } else { 0 })
