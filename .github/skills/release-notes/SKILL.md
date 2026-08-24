---
name: release-notes
description: "Clasificacion de cambios, formato de CHANGELOG y plantillas de release notes para Scato Logistica. Usar al generar entradas de changelog o notas de release para el PO."
---

# Release Notes — Scato Logistica

## Formato de versión

```
vYYYY.WW.N
  │    │  └─ patch dentro de la semana (0, 1, 2…)
  │    └──── semana ISO del año (01–53)
  └───────── año
```

Ejemplos: `v2026.35.0`, `v2026.35.1`, `v2026.35.2`

Branch asociado: `release/YYYY.WW.N`
PR merge commit: `Merged PR {id}: Merge release/YYYY.WW.N into master`

---

## Clasificación de cambios

Leer el título y descripción del PR en Azure DevOps para clasificar cada cambio:

| Categoría | Criterios | Emoji |
|---|---|---|
| `feature` | Nueva funcionalidad, nueva pantalla, nuevo flujo de workflow | ✨ |
| `fix` | Corrección de bug, manejo incorrecto de estado, error en cálculo | 🐛 |
| `afip` | Cambios en integración CPE/CTG, nuevos endpoints AFIP, cambios regulatorios | 🏛️ |
| `migration` | Cambio SQL en `Molinos.Scato.Database/` (objeto o script post-deploy) | 🗄️ |
| `performance` | Optimización de queries, reducción de N+1, mejora de tiempos | ⚡ |
| `security` | Corrección de vulnerabilidades, cambios de permisos/autorizacion | 🔒 |
| `workflow` | Cambios en `.xamlx`, nuevas actividades, refactor de WF | ⚙️ |
| `config` | Cambios en `Web.config`, `App.config`, parámetros de entorno | 🔧 |
| `chore` | Refactor interno sin impacto funcional, actualizaciones de tools | 🔨 |

### Criterios de impacto para PO

| Nivel | Descripción | Acción requerida |
|---|---|---|
| 🔴 Breaking | Cambia comportamiento existente, requiere acción del usuario | Documentar pasos de migración |
| 🟠 Notable | Nueva funcionalidad visible o corrección de bug crítico | Incluir en release notes PO |
| 🟡 Minor | Mejora pequeña o fix menor | Solo en CHANGELOG técnico |
| ⚪ Chore | Cambio interno sin impacto visible | Omitir del resumen PO |

---

## Formato CHANGELOG.md — Keep a Changelog adaptado

```markdown
# Changelog

Todas las versiones notables de Scato Logística se documentan en este archivo.
Formato basado en [Keep a Changelog](https://keepachangelog.com/es/1.0.0/).
Versiones: `vYYYY.WW.N` (año · semana ISO · patch).

---

## [v2026.35.2] — 2026-09-01

### ✨ Novedades
- Nuevo flujo de recepción de subproductos con validación automática de CTG. (PR #5721)
- Panel de monitoreo de recorridos activos con estado en tiempo real. (PR #5715)

### 🐛 Correcciones
- El pesaje bruto no se registraba cuando el operador cancelaba y reiniciaba el proceso. (PR #5718)
- Corrección en el cálculo de peso neto cuando la tara supera el bruto por tolerancia. (PR #5712)

### 🏛️ AFIP / Regulatorio
- Soporte para el nuevo campo `motivoAnulacion` requerido por AFIP desde el 01/09/2026. (PR #5720)

### 🗄️ Migraciones de base de datos
- `Molinos.Scato.Database/dbo/Tables/Recorrido.sql` — Agrega columna `MotivoAnulacion` nullable.
- `Molinos.Scato.Database/Scripts/Post-Deployment/Datos Base.sql` — Valor inicial del nuevo parámetro AFIP.

### ⚙️ Componentes afectados
- ✅ Molinos.Scato.Web
- ✅ Molinos.Scato.ServiciosWeb
- ✅ Molinos.Scato.Workflow
- ⬜ Molinos.Scato.WebMobile *(sin cambios)*
- ⬜ Molinos.Scato.WebOperaciones *(sin cambios)*
- ⬜ Molinos.Scato.ModuloImpresor *(sin cambios)*
- ⬜ Molinos.Scato.WebPuerto *(sin cambios)*
- ⬜ Molinos.Scato.WebPuertoApi *(sin cambios)*

---

## [v2026.35.1] — 2026-08-25
...
```

### Reglas del CHANGELOG
- **Siempre prepend** — la versión más nueva va al inicio, debajo del header.
- **Una sección por categoría** — omitir secciones sin entradas.
- **Cada PR = una línea** — frase corta en español, imperativo o sustantivo. Al final: `(PR #NNNN)`.
- **Scripts post-deploy de datos** — siempre describir qué dato inicializan/actualizan y su impacto operativo.
- **Fecha** en formato `YYYY-MM-DD`, derivada del tag `git log -1 --format=%ai vYYYY.WW.N`.

---

## Plantilla de Release Notes para PO

Lenguaje de negocio — sin nombres técnicos de clase, namespace ni tabla.
Audiencia: Coordinadores de planta, Supervisores, Administradores del sistema.

```markdown
# Release v{version} — Scato Logística
**Fecha:** {fecha}
**Semana:** {semana ISO}

## Qué hay de nuevo

### Para coordinadores de planta
- [descripción en lenguaje de usuario]

### Para operadores de balanza
- [si aplica]

### Para administradores del sistema
- [cambios de configuración, nuevos parámetros]

## Correcciones importantes
- [bugs corregidos con impacto visible para el usuario]

## Cambios regulatorios / AFIP
- [solo si aplica — describirlo como impacto operativo, no como cambio técnico]

## Pasos manuales requeridos
> ⚠️ Esta sección solo aparece si hay acciones requeridas post-deploy.
- [ ] Ejecutar script de datos en PROD antes de iniciar operaciones del día.
- [ ] Comunicar a los coordinadores sobre el nuevo campo X.

## Componentes que se actualizan en este deploy
- Portal principal (Web)
- Servicio de operaciones (ServiciosWeb)
- Motor de workflows (Workflow)
```

---

## Glosario de términos PO-friendly

Usar estos términos en las release notes para el PO, nunca los nombres técnicos:

| Término técnico | Término PO-friendly |
|---|---|
| `Recorrido` | viaje / recorrido de transporte |
| `CartaPorte` | carta de porte |
| `CTG` / `AltaCTG` | trámite CTG ante AFIP / constancia de transporte |
| `CPE` / `autorizarCPEAutomotor` | carta de porte electrónica (CPE) |
| `Calado` | análisis de calidad / muestreo de granos |
| `PesadaBruto` / `PesadaTara` | pesaje de entrada / pesaje de tara |
| `Calle` | muelle / bahía de carga |
| `Almacen` | silo / depósito |
| `Centro` | planta / establecimiento |
| `Workflow` / `XAMLX` | proceso / flujo operativo |
| `Procesador` / `Actividad` | lógica del sistema (omitir en PO notes) |
| `ScatoDbContext` / `Repositorio` | base de datos (omitir en PO notes) |

---

## Comandos de referencia rápida

```powershell
# Tags recientes
git --no-pager tag --sort=-creatordate | Select-Object -First 10

# Commits entre dos tags (sin merges)
git --no-pager log "v2026.35.1..v2026.35.2" --oneline --no-merges

# Solo merges de PR
git --no-pager log "v2026.35.1..v2026.35.2" --oneline --merges

# Archivos cambiados entre tags
git --no-pager diff "v2026.35.1..v2026.35.2" --name-only

# Cambios SQL incluidos (schema project + post-deploy)
git --no-pager diff "v2026.35.1..v2026.35.2" --name-only -- "Molinos.Scato.Database/"

# Fecha del tag
git --no-pager log -1 --format="%ai" "v2026.35.2"
```

```bash
# PR details via Azure DevOps CLI
az repos pr show --id {pr-id} -o json
az repos pr work-item list --id {pr-id} -o table
az repos pr list --status completed --target-branch master --top 20 -o table
```
