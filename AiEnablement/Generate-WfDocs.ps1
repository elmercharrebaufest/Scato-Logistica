#Requires -Version 5.1
<#
.SYNOPSIS
    Genera documentacion Markdown de workflows XAMLX de Scato Logistica.

.DESCRIPTION
    Extrae la estructura logica de archivos .xamlx (WF4.5), descartando metadatos
    del disenador de Visual Studio, y produce documentacion en Markdown con:
    - Descripcion del workflow
    - Variables de scope
    - Diagrama Mermaid del flujo
    - Lista de actividades personalizadas referenciadas
    - Analisis de problemas del grafo (nodos huerfanos, dead-ends)

    Modo AI: si se configura OPENAI_API_KEY o se detecta `gh copilot`, llama a la
    IA para generar descripciones en lenguaje de negocio.

.PARAMETER WorkflowPath
    Ruta al archivo .xamlx o directorio que contiene varios .xamlx.
    Por defecto: directorio Molinos.Scato.Workflow relativo al repo.

.PARAMETER OutputDir
    Directorio de salida para los archivos .md generados.
    Por defecto: Documentation\Workflows\ en la raiz del repo.

.PARAMETER UseAi
    Si se especifica, intenta generar descripciones con IA (gh copilot o Azure OpenAI).

.PARAMETER AiProvider
    Proveedor de IA a usar: 'GhCopilot' (default) | 'AzureOpenAI'.

.PARAMETER DryRun
    Muestra que se generaria sin escribir archivos.

.PARAMETER Force
    Sobreescribe archivos de documentacion existentes.

.EXAMPLE
    .\Generate-WfDocs.ps1
    # Genera docs de todos los .xamlx del repo

.EXAMPLE
    .\Generate-WfDocs.ps1 -WorkflowPath "Molinos.Scato.Workflow\Prod\SLO.IngresoPorCompraDeGranos.xamlx" -UseAi

.EXAMPLE
    .\Generate-WfDocs.ps1 -DryRun
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string]$WorkflowPath,
    [string]$OutputDir,
    [switch]$UseAi,
    [ValidateSet('GhCopilot','AzureOpenAI')]
    [string]$AiProvider = 'GhCopilot',
    [switch]$DryRun,
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Namespaces XAMLX
$NS = @{
    p      = 'http://schemas.microsoft.com/netfx/2009/xaml/activities'
    sm     = 'http://schemas.microsoft.com/netfx/2009/xaml/servicemodel'
    sap    = 'http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation'
    sap10  = 'http://schemas.microsoft.com/netfx/2010/xaml/activities/presentation'
    sads   = 'http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger'
    x      = 'http://schemas.microsoft.com/winfx/2006/xaml'
    msa    = 'clr-namespace:Molinos.Scato.Actividades;assembly=Molinos.Scato.Actividades'
}

# NSMap para Select-Xml (evita conflicto de overload con clases PS5)
$script:NSMap = @{
    p     = 'http://schemas.microsoft.com/netfx/2009/xaml/activities'
    sm    = 'http://schemas.microsoft.com/netfx/2009/xaml/servicemodel'
    sap10 = 'http://schemas.microsoft.com/netfx/2010/xaml/activities/presentation'
    sads  = 'http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger'
    x     = 'http://schemas.microsoft.com/winfx/2006/xaml'
    sap   = 'http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation'
    msa   = 'clr-namespace:Molinos.Scato.Actividades;assembly=Molinos.Scato.Actividades'
}


$DOMAIN_GLOSSARY = @"
Glosario del dominio Scato Logistica:
- Recorrido: viaje logistico de transporte de granos
- CTG: Constancia de Transito de Granos (AFIP)
- CPE: Carta de Porte Electronica (AFIP)
- Calado: muestreo fisico de calidad del grano (sonda)
- Balanza / PesadaBruto / PesadaTara: pesaje en bascula
- Calle: carril de acceso/egreso en un centro logistico
- PuntoDeCarga: punto fisico de carga del camion
- Almacen: silo o deposito
- Transportista: empresa transportista
- Chofer: conductor del camion
- Fason: operacion de procesamiento a facon (maquiladora)
- CartaPorte: documento que ampara el transporte de granos
- Centro: planta o establecimiento logistico
- Coordinador: usuario con rol de coordinacion de operaciones
- DecisionCoordinacion: decision del coordinador (Aceptar/Rechazar/Esperar)
- SAP: sistema ERP de integracion financiera/contable
- AFIP: organismo fiscal argentino
"@

# ─── RESOLUCION DE RUTAS ──────────────────────────────────────────────────────

function Resolve-RepoPaths {
    $scriptDir = Split-Path -Parent $PSCommandPath
    $repoRoot  = $scriptDir
    while ($repoRoot -and -not (Test-Path (Join-Path $repoRoot 'Molinos.Scato.sln'))) {
        $parent = Split-Path -Parent $repoRoot
        if ($parent -eq $repoRoot) {
            throw 'No se pudo resolver la raiz del repo (Molinos.Scato.sln).'
        }

        $repoRoot = $parent
    }

    if (-not $WorkflowPath) {
        $script:ResolvedWorkflowPath = Join-Path $repoRoot 'Molinos.Scato.Workflow'
    } else {
        $script:ResolvedWorkflowPath = $WorkflowPath
        if (-not [System.IO.Path]::IsPathRooted($WorkflowPath)) {
            $script:ResolvedWorkflowPath = Join-Path (Get-Location) $WorkflowPath
        }
    }

    if (-not $OutputDir) {
        $script:ResolvedOutputDir = Join-Path $repoRoot 'Documentation\Workflows'
    } else {
        $script:ResolvedOutputDir = $OutputDir
    }
}

# ─── HELPERS XML ─────────────────────────────────────────────────────────────

function New-NsMgr($doc) {
    $mgr = [System.Xml.XmlNamespaceManager]::new($doc.NameTable)
    foreach ($k in $NS.Keys) { $mgr.AddNamespace($k, $NS[$k]) }
    return $mgr
}

function Remove-DesignerNoise($doc, $mgr) {
    $mgr = New-NsMgr $doc
    $xpaths = @(
        '//sap10:WorkflowViewState.ViewStateManager',
        '//sads:DebugSymbol.Symbol'
    )
    foreach ($xp in $xpaths) {
        $nodes = @(Select-Xml -Xml $doc -XPath $xp -Namespace $script:NSMap | ForEach-Object { $_.Node })
        foreach ($n in $nodes) {
            $n.ParentNode.RemoveChild($n) | Out-Null
        }
    }
    return $doc
}

# ─── MODELOS ─────────────────────────────────────────────────────────────────

class WfActivity {
    [string]$LocalName
    [string]$DisplayName
    [System.Collections.Hashtable]$Arguments
    WfActivity() { $this.Arguments = @{} }
}

class WfFlowDecision {
    [string]$Id
    [string]$DisplayName
    [string]$Condition
}

class WfTransition {
    [string]$DisplayName
    [string]$To
    [string]$Condition
    [string]$Trigger
}

class WfState {
    [string]$Name
    [bool]$IsFinal
    [System.Collections.Generic.List[WfActivity]]$EntryActivities
    [System.Collections.Generic.List[WfTransition]]$Transitions
    WfState() {
        $this.IsFinal         = $false
        $this.EntryActivities = [System.Collections.Generic.List[WfActivity]]::new()
        $this.Transitions     = [System.Collections.Generic.List[WfTransition]]::new()
    }
}

class WfVariable {
    [string]$Name
    [string]$Type
    [string]$Default
}

class XamlxModel {
    [string]$FileName
    [string]$ConfigurationName
    [string]$RootActivityType
    [System.Collections.Generic.List[WfVariable]]$Variables
    [System.Collections.Generic.List[WfActivity]]$FlowActivities
    [System.Collections.Generic.List[WfFlowDecision]]$FlowDecisions
    [System.Collections.Generic.List[WfState]]$States
    [string[]]$CustomActivities
    [string[]]$ReferencedAssemblies
    [string[]]$GraphProblems
    XamlxModel() {
        $this.Variables       = [System.Collections.Generic.List[WfVariable]]::new()
        $this.FlowActivities  = [System.Collections.Generic.List[WfActivity]]::new()
        $this.FlowDecisions   = [System.Collections.Generic.List[WfFlowDecision]]::new()
        $this.States          = [System.Collections.Generic.List[WfState]]::new()
        $this.CustomActivities    = @()
        $this.ReferencedAssemblies = @()
        $this.GraphProblems       = @()
    }
}

# ─── EXTRACCION ──────────────────────────────────────────────────────────────

function Extract-Variables($doc, $mgr) {
    $vars = [System.Collections.Generic.List[WfVariable]]::new()
    foreach ($v in @(Select-Xml -Xml $doc -XPath '//p:Variable' -Namespace $script:NSMap | ForEach-Object { $_.Node })) {
        # TypeArguments puede venir como atributo con namespace x o como texto plano
        $ta = $v.GetAttribute('TypeArguments', $NS.x)
        if (-not $ta) { $ta = $v.GetAttribute('x:TypeArguments') }
        if (-not $ta) {
            # buscar en todos los atributos
            foreach ($attr in $v.Attributes) {
                if ($attr.LocalName -eq 'TypeArguments') { $ta = $attr.Value; break }
            }
        }
        $wfv = [WfVariable]::new()
        $wfv.Type    = ($ta -replace '^[a-z]+:','') -replace '^x:',''
        $wfv.Name    = $v.GetAttribute('Name')
        $wfv.Default = $v.GetAttribute('Default')
        if ($wfv.Name) { $vars.Add($wfv) }
    }
    return $vars
}

function Extract-Flowchart($doc, $mgr, $model) {
    $model.RootActivityType = 'Flowchart'
    $MSA_NS = $NS.msa

    foreach ($step in @(Select-Xml -Xml $doc -XPath '//p:FlowStep' -Namespace $script:NSMap | ForEach-Object { $_.Node })) {
        foreach ($child in $step.ChildNodes) {
            if ($child.NodeType -ne [System.Xml.XmlNodeType]::Element) { continue }
            if ($child.LocalName -match '^(FlowStep|FlowDecision|FlowSwitch|Next)$') { continue }
            if ($child.NamespaceURI -eq $MSA_NS -or
                ($child.NamespaceURI -and $child.NamespaceURI -notmatch 'schemas\.microsoft\.com/(netfx|winfx)')) {
                $act = [WfActivity]::new()
                $act.LocalName   = $child.LocalName
                $act.DisplayName = $child.GetAttribute('DisplayName')
                if (-not $act.DisplayName) { $act.DisplayName = $child.LocalName }
                foreach ($attr in $child.Attributes) {
                    if ($attr.NamespaceURI -notmatch 'sap|sads|debugger|presentation') {
                        $act.Arguments[$attr.LocalName] = $attr.Value
                    }
                }
                $model.FlowActivities.Add($act)
            }
        }
    }

    foreach ($d in @(Select-Xml -Xml $doc -XPath '//p:FlowDecision' -Namespace $script:NSMap | ForEach-Object { $_.Node })) {
        $fd = [WfFlowDecision]::new()
        $fd.Id          = $d.GetAttribute('Name', $NS.x)
        $fd.DisplayName = $d.GetAttribute('DisplayName')
        $fd.Condition   = $d.GetAttribute('Condition')
        if (-not $fd.DisplayName) { $fd.DisplayName = $fd.Condition }
        $model.FlowDecisions.Add($fd)
    }
}

function Extract-StateMachine($doc, $mgr, $model) {
    $model.RootActivityType = 'StateMachine'

    foreach ($sn in @(Select-Xml -Xml $doc -XPath '//p:State' -Namespace $script:NSMap | ForEach-Object { $_.Node })) {
        $state = [WfState]::new()
        $state.Name    = $sn.GetAttribute('DisplayName')
        if (-not $state.Name) { $state.Name = $sn.GetAttribute('Name', $NS.x) }
        if (-not $state.Name) { $state.Name = 'Estado desconocido' }
        $state.IsFinal = ($sn.GetAttribute('IsFinal') -eq 'True')

        $entryNodeR = Select-Xml -Xml $sn -XPath 'p:State.Entry' -Namespace $script:NSMap | Select-Object -First 1
        $entryNode = if ($entryNodeR) { $entryNodeR.Node } else { $null }
        if ($entryNode) {
            foreach ($child in $entryNode.ChildNodes) {
                if ($child.NodeType -ne [System.Xml.XmlNodeType]::Element) { continue }
                $act = [WfActivity]::new()
                $act.LocalName   = $child.LocalName
                $act.DisplayName = $child.GetAttribute('DisplayName')
                if (-not $act.DisplayName) { $act.DisplayName = $child.LocalName }
                $state.EntryActivities.Add($act)
            }
        }

        foreach ($tn in @(Select-Xml -Xml $sn -XPath 'p:State.Transitions/p:Transition' -Namespace $script:NSMap | ForEach-Object { $_.Node })) {
            $tr = [WfTransition]::new()
            $tr.DisplayName = $tn.GetAttribute('DisplayName')
            $tr.To          = $tn.GetAttribute('To')
            $condNodeR = Select-Xml -Xml $tn -XPath 'p:Transition.Condition' -Namespace $script:NSMap | Select-Object -First 1
            $condNode = if ($condNodeR) { $condNodeR.Node } else { $null }
            $trigNodeR = Select-Xml -Xml $tn -XPath 'p:Transition.Trigger' -Namespace $script:NSMap | Select-Object -First 1
            $trigNode = if ($trigNodeR) { $trigNodeR.Node } else { $null }
            $tr.Condition = if ($condNode) { $condNode.InnerText.Trim() } else { '' }
            $tr.Trigger   = if ($trigNode) { 'si' } else { '' }
            $state.Transitions.Add($tr)
        }

        $model.States.Add($state)
    }
}

function Resolve-CustomActivities($doc, $model) {
    $seen = [System.Collections.Generic.HashSet[string]]::new()
    $MSA_NS = $NS.msa
    foreach ($node in $doc.GetElementsByTagName('*')) {
        if ($node.NamespaceURI -eq $MSA_NS) {
            [void]$seen.Add($node.LocalName)
        }
    }
    $model.CustomActivities = @($seen | Sort-Object)
}

function Analyze-GraphProblems($model) {
    $problems = [System.Collections.Generic.List[string]]::new()

    if ($model.RootActivityType -eq 'StateMachine' -and $model.States.Count -gt 0) {
        $allTo = $model.States | ForEach-Object { $_.Transitions } | ForEach-Object { $_.To } | Where-Object { $_ }
        $firstState = $model.States[0].Name

        foreach ($s in $model.States) {
            if (-not $s.IsFinal -and $s.Transitions.Count -eq 0) {
                $problems.Add("Estado '$($s.Name)' no tiene transiciones salientes y no esta marcado como final.")
            }
        }
        foreach ($s in $model.States | Select-Object -Skip 1) {
            if ($s.Name -notin $allTo) {
                $problems.Add("Estado '$($s.Name)' no tiene transiciones entrantes (posible huerfano).")
            }
        }
        foreach ($s in $model.States) {
            $sinCond = @($s.Transitions | Where-Object { -not $_.Condition })
            if ($s.Transitions.Count -gt 1 -and $sinCond.Count -gt 0) {
                $problems.Add("Estado '$($s.Name)' tiene $($s.Transitions.Count) transiciones pero alguna sin condicion (no-determinista).")
            }
        }
    }

    if ($model.RootActivityType -eq 'Flowchart' -and $model.FlowActivities.Count -eq 0 -and $model.FlowDecisions.Count -eq 0) {
        $problems.Add("Flowchart sin actividades reconocidas.")
    }

    $model.GraphProblems = @($problems)
}

function Extract-XamlxModel($xamlxPath) {
    Write-Verbose "Parseando: $xamlxPath"
    [xml]$doc = Get-Content $xamlxPath -Encoding UTF8 -Raw
    $mgr = New-NsMgr $doc

    $model = [XamlxModel]::new()
    $model.FileName          = [System.IO.Path]::GetFileNameWithoutExtension($xamlxPath)
    $model.ConfigurationName = $doc.DocumentElement.GetAttribute('ConfigurationName')
    if (-not $model.ConfigurationName) { $model.ConfigurationName = $model.FileName }

    Remove-DesignerNoise $doc $mgr | Out-Null

    # PS5 bug: assigning empty List<T> from function return sets property to $null
    # Use AddRange pattern to avoid re-assignment of empty lists
    foreach ($v in @(Extract-Variables $doc $mgr)) { $model.Variables.Add($v) }

    $model.ReferencedAssemblies = @(
        @(Select-Xml -Xml $doc -XPath '//p:AssemblyReference' -Namespace $script:NSMap | ForEach-Object { $_.Node }) |
        ForEach-Object { $_.InnerText } |
        Where-Object { $_ -match 'Molinos' } |
        Sort-Object -Unique
    )

    $smNodeR = Select-Xml -Xml $doc -XPath '//p:StateMachine' -Namespace $script:NSMap | Select-Object -First 1
    $smNode = if ($smNodeR) { $smNodeR.Node } else { $null }
    if ($smNode) {
        Extract-StateMachine $doc $mgr $model
    } else {
        Extract-Flowchart $doc $mgr $model
    }

    Resolve-CustomActivities $doc $model
    Analyze-GraphProblems $model

    return $model
}

# ─── MERMAID ─────────────────────────────────────────────────────────────────

function Sanitize-MermaidId($name) {
    $id = $name -replace '[^a-zA-Z0-9]', '_'
    $id = $id -replace '^[0-9_]+', 'N'
    return $id
}

function Build-MermaidFlowchart($model) {
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('```mermaid')
    $lines.Add('flowchart TD')

    $nodeIds = @{}
    $idx = 0

    foreach ($act in $model.FlowActivities) {
        $id = "A$idx"
        $nodeIds[$act.DisplayName] = $id
        $label = ($act.DisplayName -replace '"',"'") -replace '[\[\]]',''
        $lines.Add("    $id[$label]")
        $idx++
    }
    foreach ($dec in $model.FlowDecisions) {
        $id = "D$idx"
        $dn = if ($dec.DisplayName) { $dec.DisplayName } else { "Decision$idx" }
        $nodeIds[$dn] = $id
        $cond = ($dec.Condition -replace '[\[\]]','') -replace '"',"'"
        if (-not $cond) { $cond = $dn }
        $lines.Add("    $id{`"$cond`"}")
        $idx++
    }

    # Aristas entre actividades secuenciales
    for ($i = 0; $i -lt $model.FlowActivities.Count - 1; $i++) {
        $from = $nodeIds[$model.FlowActivities[$i].DisplayName]
        $to   = $nodeIds[$model.FlowActivities[$i+1].DisplayName]
        if ($from -and $to) { $lines.Add("    $from --> $to") }
    }

    $lines.Add('```')
    return $lines -join "`n"
}

function Build-MermaidStateMachine($model) {
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('```mermaid')
    $lines.Add('stateDiagram-v2')

    if ($model.States.Count -gt 0) {
        $firstId = Sanitize-MermaidId $model.States[0].Name
        $lines.Add("    [*] --> $firstId")
    }

    foreach ($state in $model.States) {
        $sid = Sanitize-MermaidId $state.Name
        if ($state.IsFinal) {
            $lines.Add("    $sid --> [*]")
        }
        foreach ($tr in $state.Transitions) {
            if (-not $tr.To) { continue }
            $toId = Sanitize-MermaidId $tr.To
            $label = if ($tr.DisplayName) { $tr.DisplayName }
                     elseif ($tr.Condition) { $tr.Condition -replace '"',"'" -replace '[\[\]]','' }
                     else { '' }
            if ($label.Length -gt 60) { $label = $label.Substring(0,57) + '...' }
            if ($label) {
                $lines.Add("    $sid --> $toId : $label")
            } else {
                $lines.Add("    $sid --> $toId")
            }
        }
    }

    $lines.Add('```')
    return $lines -join "`n"
}

# ─── INTEGRACION IA ───────────────────────────────────────────────────────────

function Build-AiPrompt($model) {
    $actsList = ($model.CustomActivities | Select-Object -First 20) -join ', '
    $varsList  = ($model.Variables | Select-Object -First 12 | ForEach-Object { "$($_.Name):$($_.Type)" }) -join ', '

    $stateContext = ''
    if ($model.States.Count -gt 0) {
        $stateContext = "`nEstados: $(($model.States | ForEach-Object { $_.Name }) -join ' -> ')"
    }

    $decisionContext = ''
    if ($model.FlowDecisions.Count -gt 0) {
        $sample = $model.FlowDecisions | Select-Object -First 10 | ForEach-Object {
            $cond = if ($_.Condition) { " [si: $($_.Condition)]" } else { '' }
            "$($_.DisplayName)$cond"
        }
        $decisionContext = "`nDecisiones de flujo: " + ($sample -join '; ')
    }
    if ($model.States.Count -gt 0 -and $model.States[0].Transitions.Count -gt 0) {
        $trs = $model.States | ForEach-Object { $st = $_; $st.Transitions | ForEach-Object { "$($st.Name) -[$($_.Trigger)]-> $($_.To)" } }
        $decisionContext += "`nTransiciones: " + (($trs | Select-Object -First 10) -join '; ')
    }

    $warnings = ''
    if ($model.GraphProblems.Count -gt 0) {
        $warnings = "`nAlertas: $(($model.GraphProblems) -join '; ')"
    }

    return @"
$DOMAIN_GLOSSARY

Analiza el workflow WF4.5 de Scato Logistica y responde SOLO con Markdown estructurado en estos tres encabezados:

### Descripcion de negocio
2-3 parrafos: que hace, cuando se dispara, actores involucrados.

### Etapas principales
Lista numerada en lenguaje de negocio (no tecnico).

### Reglas de negocio
Bullet list con validaciones o decisiones criticas.

---
Workflow: $($model.ConfigurationName)
Tipo: $($model.RootActivityType)
Variables: $varsList
Actividades custom: $actsList$stateContext$decisionContext$warnings

Sin bloques de codigo envolventes. Solo los tres encabezados indicados.
"@
}

function Get-AiDescription($model) {
    if (-not $UseAi) { return $null }

    $prompt = Build-AiPrompt $model

    if ($AiProvider -eq 'AzureOpenAI') {
        return Invoke-AzureOpenAI $prompt
    }
    return Invoke-GhCopilot $prompt
}

function Invoke-GhCopilot($prompt) {
    if (-not (Get-Command 'gh' -ErrorAction SilentlyContinue)) {
        Write-Warning "gh CLI no encontrado. Omitiendo descripcion AI."
        return $null
    }

    # Obtener token OAuth de gh CLI
    $token = (& gh auth token 2>&1) | Where-Object { $_ -match '^gho_|^ghp_|^github_pat_' } | Select-Object -First 1
    if (-not $token) {
        # gh auth token puede devolver solo la linea del token sin prefijo en algunas versiones
        $token = (& gh auth token 2>&1) -join '' | ForEach-Object { $_.Trim() }
    }
    if (-not $token) {
        Write-Warning "No se pudo obtener token de gh CLI. Ejecuta: gh auth login"
        return $null
    }

    $body = @{
        model    = 'gpt-4o-mini'
        messages = @(
            @{
                role    = 'system'
                content = 'Eres arquitecto de software experto en WF4.5 y logistica de granos (Argentina). Documenta en espanol tecnico, preciso y sin ambiguedades.'
            }
            @{
                role    = 'user'
                content = $prompt
            }
        )
        max_tokens  = 900
        temperature = 0.2
    } | ConvertTo-Json -Depth 6

    try {
        $resp = Invoke-RestMethod `
            -Uri     'https://api.githubcopilot.com/chat/completions' `
            -Method  POST `
            -Headers @{
                'Authorization'          = "Bearer $token"
                'Content-Type'           = 'application/json'
                'Copilot-Integration-Id' = 'vscode-chat'
            } `
            -Body $body
        return $resp.choices[0].message.content
    } catch {
        Write-Warning "Error GitHub Copilot API: $($_.Exception.Message)"
        return $null
    }
}

function Invoke-AzureOpenAI($prompt) {
    $apiKey  = $env:OPENAI_API_KEY
    $apiBase = $env:AZURE_OPENAI_ENDPOINT
    $deploy  = $env:AZURE_OPENAI_DEPLOYMENT

    if (-not $apiKey -or -not $apiBase -or -not $deploy) {
        Write-Warning "Variables OPENAI_API_KEY / AZURE_OPENAI_ENDPOINT / AZURE_OPENAI_DEPLOYMENT no configuradas."
        return $null
    }

    $body = @{
        messages    = @(
            @{ role = 'system'; content = 'Eres arquitecto de software experto en WF4.5 y logistica de granos (Argentina). Documenta en espanol tecnico.' }
            @{ role = 'user';   content = $prompt }
        )
        max_tokens  = 800
        temperature = 0.2
    } | ConvertTo-Json -Depth 5

    $uri = "$apiBase/openai/deployments/$deploy/chat/completions?api-version=2024-02-01"
    try {
        $resp = Invoke-RestMethod -Uri $uri -Method POST -Headers @{
            'api-key'      = $apiKey
            'Content-Type' = 'application/json'
        } -Body $body
        return $resp.choices[0].message.content
    } catch {
        Write-Warning "Error Azure OpenAI: $_"
        return $null
    }
}

# ─── RENDER MARKDOWN ─────────────────────────────────────────────────────────

function Render-WorkflowDoc($model) {
    $now   = Get-Date -Format 'yyyy-MM-dd HH:mm'
    $lines = [System.Collections.Generic.List[string]]::new()

    $lines.Add("# Workflow: $($model.ConfigurationName)")
    $lines.Add('')
    $lines.Add("> **Archivo:** ``$($model.FileName).xamlx``  ")
    $lines.Add("> **Tipo de raiz:** $($model.RootActivityType)  ")
    $lines.Add("> **Generado:** $now  ")
    $lines.Add("> **Actividades custom:** $($model.CustomActivities.Count)  ")
    $lines.Add("> **Variables de scope:** $($model.Variables.Count)  ")
    $lines.Add('')

    # Descripcion de negocio
    $lines.Add('## Descripcion de negocio')
    $lines.Add('')
    $aiText = Get-AiDescription $model
    if ($aiText) {
        $lines.Add($aiText)
    } else {
        $lines.Add('_TODO: Agregar descripcion del proceso de negocio._')
        if ($UseAi) {
            $lines.Add('')
            $lines.Add('> **Nota:** La descripcion AI no pudo generarse (revisar configuracion del proveedor).')
        }
    }
    $lines.Add('')

    # Variables
    if ($model.Variables.Count -gt 0) {
        $lines.Add('## Variables de scope')
        $lines.Add('')
        $lines.Add('| Nombre | Tipo | Valor por defecto |')
        $lines.Add('|--------|------|-------------------|')
        foreach ($v in $model.Variables | Sort-Object Name) {
            $def = if ($v.Default) { $v.Default } else { '--' }
            $lines.Add("| ``$($v.Name)`` | ``$($v.Type)`` | $def |")
        }
        $lines.Add('')
    }

    # Diagrama
    $lines.Add('## Diagrama del flujo')
    $lines.Add('')
    if ($model.RootActivityType -eq 'StateMachine' -and $model.States.Count -gt 0) {
        $lines.Add((Build-MermaidStateMachine $model))
    } elseif ($model.FlowActivities.Count -gt 0 -or $model.FlowDecisions.Count -gt 0) {
        $lines.Add((Build-MermaidFlowchart $model))
    } else {
        $lines.Add('_Diagrama no disponible._')
    }
    $lines.Add('')

    # Estados (StateMachine)
    if ($model.RootActivityType -eq 'StateMachine' -and $model.States.Count -gt 0) {
        $lines.Add('## Estados y transiciones')
        $lines.Add('')
        foreach ($state in $model.States) {
            $badge = if ($state.IsFinal) { ' _(final)_' } else { '' }
            $lines.Add("### $($state.Name)$badge")
            $lines.Add('')
            if ($state.EntryActivities.Count -gt 0) {
                $lines.Add('**Actividades de entrada:**')
                foreach ($a in $state.EntryActivities) {
                    $lines.Add("- ``$($a.LocalName)``")
                }
                $lines.Add('')
            }
            if ($state.Transitions.Count -gt 0) {
                $lines.Add('**Transiciones:**')
                $lines.Add('')
                $lines.Add('| Destino | Condicion | Tipo |')
                $lines.Add('|---------|-----------|------|')
                foreach ($tr in $state.Transitions) {
                    $cond = if ($tr.Condition) { $tr.Condition } else { '_siempre_' }
                    $tipo = if ($tr.Trigger)   { 'con trigger' } else { 'automatica' }
                    $dest = if ($tr.To)        { $tr.To } else { '--' }
                    $lines.Add("| $dest | $cond | $tipo |")
                }
                $lines.Add('')
            }
        }
    }

    # Actividades Flowchart
    if ($model.RootActivityType -eq 'Flowchart' -and $model.FlowActivities.Count -gt 0) {
        $lines.Add('## Secuencia de actividades')
        $lines.Add('')
        $lines.Add('| # | Actividad | Argumentos clave |')
        $lines.Add('|---|-----------|-----------------|')
        $i = 1
        foreach ($act in $model.FlowActivities) {
            $argKeys = ($act.Arguments.Keys | Where-Object { $_ -notmatch '^(DisplayName|Name)$' } | Select-Object -First 5) -join ', '
            $lines.Add("| $i | ``$($act.LocalName)`` | $argKeys |")
            $i++
        }
        $lines.Add('')
        if ($model.FlowDecisions.Count -gt 0) {
            $lines.Add('**Decisiones de flujo:**')
            $lines.Add('')
            foreach ($dec in $model.FlowDecisions) {
                $cond = if ($dec.Condition) { "``$($dec.Condition)``" } else { '_sin condicion_' }
                $dn   = if ($dec.DisplayName) { $dec.DisplayName } else { 'Decision' }
                $lines.Add("- **$dn**: $cond")
            }
            $lines.Add('')
        }
    }

    # Actividades custom
    if ($model.CustomActivities.Count -gt 0) {
        $lines.Add('## Actividades custom referenciadas')
        $lines.Add('')
        $lines.Add('> Ensamblado: ``Molinos.Scato.Actividades``')
        $lines.Add('')
        foreach ($a in $model.CustomActivities) {
            $lines.Add("- ``$a``")
        }
        $lines.Add('')
    }

    # Dependencias de dominio
    if ($model.ReferencedAssemblies.Count -gt 0) {
        $lines.Add('## Dependencias de dominio')
        $lines.Add('')
        foreach ($asm in $model.ReferencedAssemblies) {
            $lines.Add("- ``$asm``")
        }
        $lines.Add('')
    }

    # Problemas del grafo
    if ($model.GraphProblems.Count -gt 0) {
        $lines.Add('## Problemas detectados en el grafo')
        $lines.Add('')
        foreach ($p in $model.GraphProblems) {
            $lines.Add("- WARNING: $p")
        }
        $lines.Add('')
    }

    $lines.Add('---')
    $lines.Add("_Documentacion generada automaticamente por ``AiEnablement/Generate-WfDocs.ps1``._")

    return $lines -join "`n"
}

# ─── INDICE ───────────────────────────────────────────────────────────────────

function Render-Index([XamlxModel[]]$models) {
    $now   = Get-Date -Format 'yyyy-MM-dd HH:mm'
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('# Indice de Workflows — Scato Logistica')
    $lines.Add('')
    $lines.Add("> Generado: $now | Total: $($models.Count) workflows")
    $lines.Add('')
    $lines.Add('| Workflow | Tipo | Variables | Actividades | Problemas |')
    $lines.Add('|----------|------|-----------|-------------|-----------|')
    foreach ($m in $models | Sort-Object FileName) {
        $prob = if ($m.GraphProblems.Count -gt 0) { "WARNING $($m.GraphProblems.Count)" } else { 'OK' }
        $lines.Add("| [$($m.FileName)](./$($m.FileName).md) | $($m.RootActivityType) | $($m.Variables.Count) | $($m.CustomActivities.Count) | $prob |")
    }
    $lines.Add('')
    $lines.Add('---')
    $lines.Add("_Generado por ``AiEnablement/Generate-WfDocs.ps1``._")
    return $lines -join "`n"
}

# ─── MAIN ─────────────────────────────────────────────────────────────────────

function Main {
    Resolve-RepoPaths

    # Recolectar archivos
    if (Test-Path $script:ResolvedWorkflowPath -PathType Leaf) {
        $xamlxFiles = @(Get-Item $script:ResolvedWorkflowPath)
    } elseif (Test-Path $script:ResolvedWorkflowPath -PathType Container) {
        $xamlxFiles = @(Get-ChildItem $script:ResolvedWorkflowPath -Recurse -Filter '*.xamlx')
    } else {
        Write-Error "La ruta no existe: $script:ResolvedWorkflowPath"
        exit 1
    }

    if ($xamlxFiles.Count -eq 0) {
        Write-Warning "No se encontraron archivos .xamlx en: $script:ResolvedWorkflowPath"
        return
    }

    Write-Host "Procesando $($xamlxFiles.Count) workflow(s)..." -ForegroundColor Cyan

    if (-not $DryRun -and -not (Test-Path $script:ResolvedOutputDir)) {
        New-Item -ItemType Directory -Path $script:ResolvedOutputDir -Force | Out-Null
        Write-Host "Directorio creado: $script:ResolvedOutputDir" -ForegroundColor Gray
    }

    $allModels = [System.Collections.Generic.List[XamlxModel]]::new()
    $ok = 0; $skipped = 0; $errors = 0

    foreach ($file in $xamlxFiles) {
        Write-Host "  > $($file.Name)" -NoNewline
        try {
            $model = Extract-XamlxModel $file.FullName
            $outFile = Join-Path $script:ResolvedOutputDir "$($model.FileName).md"

            if (-not $Force -and -not $DryRun -and (Test-Path $outFile)) {
                Write-Host " [omitido — ya existe, usar -Force]" -ForegroundColor Yellow
                $skipped++
                $allModels.Add($model)
                continue
            }

            $md = Render-WorkflowDoc $model

            if ($DryRun) {
                Write-Host " [DryRun] $outFile" -ForegroundColor Cyan
                $md -split "`n" | Select-Object -First 6 | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkGray }
            } else {
                [System.IO.File]::WriteAllText($outFile, $md, [System.Text.Encoding]::UTF8)
                Write-Host " OK -> $([System.IO.Path]::GetFileName($outFile))" -ForegroundColor Green
            }

            if ($model.GraphProblems.Count -gt 0) {
                foreach ($p in $model.GraphProblems) {
                    Write-Host "    [!] $p" -ForegroundColor Yellow
                }
            }

            $allModels.Add($model)
            $ok++
        } catch {
            Write-Host " ERROR: $_" -ForegroundColor Red
            $errors++
        }
    }

    # Indice
    if ($allModels.Count -gt 1) {
        $indexFile = Join-Path $script:ResolvedOutputDir '_index.md'
        $idxMd     = Render-Index @($allModels)
        if ($DryRun) {
            Write-Host "`n[DryRun] Indice -> $indexFile" -ForegroundColor Cyan
        } else {
            [System.IO.File]::WriteAllText($indexFile, $idxMd, [System.Text.Encoding]::UTF8)
            Write-Host "`nIndice generado: $indexFile" -ForegroundColor Cyan
        }
    }

    Write-Host "`n────────────────────────────────────────────────"
    Write-Host "OK: $ok  |  Omitidos: $skipped  |  Errores: $errors" -ForegroundColor $(if ($errors -gt 0) { 'Red' } else { 'Green' })
    if (-not $DryRun -and $ok -gt 0) {
        Write-Host "Documentacion en: $script:ResolvedOutputDir" -ForegroundColor Cyan
    }
}

Main