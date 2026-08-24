# AGENTS.md — Molinos Scato Logística

> **Para agentes autónomos de GitHub Copilot.**
> Este archivo es el punto de entrada obligatorio para todos los agentes custom.
> Contiene el mapa de capas, convenciones de naming, rutas exactas por artefacto, entry points por feature y restricciones críticas del stack.

Enterprise agricultural logistics platform (Argentina) built on .NET Framework 4.5.2. Manages grain transport, AFIP regulatory compliance (CPE/CTG cargo manifests), quality analysis, weighing, and port/warehouse operations.

## Solution Structure

22-project Visual Studio solution organized in tiers:

| Folder | Purpose |
|--------|---------|
| `Molinos.Scato.Dominio/` | Domain entities (`Entidades/`), DTOs (`Dto/`), enums, queries, validations |
| `Molinos.Scato.Repositorio/` | EF5 DbContext (`ScatoDbContext`), `IRepositorio<T>`, `IComando`, `IConsulta`, `IConsultaPaginada` |
| `Molinos.Scato.Servicios/` | WCF service implementations — AFIP, SAP, quality, Hangfire async jobs |
| `Molinos.Scato.ServiciosWeb/` | WCF service host |
| `Molinos.Scato.Actividades/` | 40+ Windows Workflow Foundation 4.5 custom activities (`.xaml` o `.cs`) |
| `Molinos.Scato.Workflow/` | WF4.5 service host with `.xamlx` workflow state machines |
| `Molinos.Scato.Web/` | Primary ASP.NET MVC 4 portal |
| `Molinos.Scato.WebMobile/` | Mobile/tablet UI |
| `Molinos.Scato.WebOperaciones/` | Operations dashboard |
| `Molinos.Scato.WebPuertoApi/` | ASP.NET Web API 2 for port operations |
| `Molinos.Scato.WfEditorWeb/` | Web-based workflow editor |
| `Molinos.Scato.Migrations/` | Legacy SQL migration scripts (referencia histórica) |
| `Molinos.Scato.Database/` | SQL Server Data Tools project (`.sqlproj`) |
| `Molinos.Scato.Reportes/` | SSRS reports (`.rptproj`, `.rdl`) |
| `Molinos.Scato.Test/` | NUnit tests |
| `Molinos.Scato.Build/` | MSBuild orchestration (`build.proj`, `build.targets`) |

## Rutas exactas por artefacto

| Artefacto | Ruta |
|---|---|
| Entidad | `Molinos.Scato.Dominio/Entidades/{Entidad}.cs` |
| DTO | `Molinos.Scato.Dominio/Dto/{Entidad}Dto.cs` |
| Comando | `Molinos.Scato.Dominio/Comandos/{Verbo}{Entidad}.cs` |
| Textos (ES) | `Molinos.Scato.Dominio/Recursos/Textos.resx` |
| Textos (EN) | `Molinos.Scato.Dominio/Recursos/Textos.en.resx` |
| Validador de formato | `Molinos.Scato.Dominio/Validations/{Entidad}Validator.cs` |
| Procesador | `Molinos.Scato.Servicios/Procesamiento/Procesador{Verbo}{Entidad}.cs` |
| Consulta EF paginada | `Molinos.Scato.Repositorio/ConsultasEF/{Nombre}Consulta.cs` |
| Mapping Profile | `Molinos.Scato.Servicios/Conversiones/{Entidad}MappingProfile.cs` |
| Actividad pública | `Molinos.Scato.Actividades/{NombreActividad}.cs` |
| Actividad interna | `Molinos.Scato.Actividades/Internas/{NombreActividad}.cs` |
| Workflow XAMLX | `Molinos.Scato.Workflow/Prod/{Nombre}.xamlx` |
| DI bindings | `Molinos.Scato.Dependencias/{Host}NinjectModule.cs` |
| Objeto SQL (tabla/vista/SP) | `Molinos.Scato.Database/dbo/{Tables|Views|Stored Procedures}/{Objeto}.sql` |
| Script post-deploy | `Molinos.Scato.Database/Scripts/Post-Deployment/{Nombre}.sql` |
| Test procesador | `Molinos.Scato.Test/Procesamiento/Procesador{Verbo}{Entidad}Test.cs` |
| Test actividad | `Molinos.Scato.Test/Actividades/{NombreActividad}Test.cs` |
| Test controller | `Molinos.Scato.Test/Controllers/{NombreController}Test.cs` |
| Fixture de test | `Molinos.Scato.Test/FactoryContext.cs` |
| Documentación workflow | `Documentation/Workflows/{NombreWorkflow}.md` |

---

## Entry points por feature — flujo de implementación

### Agregar un caso de uso (ABM u operación de negocio)
Ejecutar en este orden:
1. **DTO**: crear/actualizar en `Dominio/Dto/`.
2. **Comando**: crear en `Dominio/Comandos/` heredando de `Comando`.
3. **Textos**: agregar claves nuevas en `Textos.resx` + `Textos.en.resx` si hay mensajes nuevos.
4. **Validador**: crear o actualizar `{Entidad}Validator.cs` en `Dominio/Validations/` para reglas sin DB.
5. **Procesador**: crear `Procesador{Verbo}{Entidad}.cs` en `Servicios/Procesamiento/`.
6. **Consulta EF** *(solo si hay lectura compleja)*: crear en `Repositorio/ConsultasEF/`.
7. **DI**: registrar procesador y validador en `Dependencias/{Host}NinjectModule.cs`.
8. **Web**: agregar acción en controller usando `IServicioComandos` / `IServicioRepositorio`.
9. **Test**: crear `Procesador{Verbo}{Entidad}Test.cs` en `Test/Procesamiento/`.

### Agregar o modificar un campo en base de datos
1. **Schema project**: actualizar el objeto en `Molinos.Scato.Database/dbo/Tables/{Tabla}.sql` (fuente de verdad para Publish).
2. **Entidad**: agregar propiedad en `Dominio/Entidades/`.
3. **DTO + Mapping**: actualizar DTO y `{Entidad}MappingProfile.cs` si el campo se expone en UI.
4. **DbContext**: si existe `EntityTypeConfiguration` explícita para la tabla, actualizarla en `ScatoDbContext`.
5. **Datos de referencia/backfill (si aplica)**: ajustar script en `Molinos.Scato.Database/Scripts/Post-Deployment/` y su inclusión en `.sqlproj`.

### Crear o modificar una actividad WF
1. Leer `.github/instructions/actividades.instructions.md` **antes de escribir código**.
2. Si la actividad supera 150 líneas o tiene 3+ `GetExtension<T>()` → invocar agente `wf-activity-refactor`.
3. Nunca modificar `.xamlx` directamente — solo los `.cs` de actividades.
4. Nunca cambiar `InArgument`/`OutArgument` públicos de un orquestador — los `.xamlx` los referencian por nombre.

### Implementar operación AFIP (CPE/CTG)
1. Leer `.github/skills/afip-cpe-ctg/SKILL.md` — tabla de operaciones, campos, patrones de error.
2. Crear procesador en `Servicios/Procesamiento/` que wrappea el proxy AFIP.
3. Mapear todos los errores AFIP a `Resultado.Errores` antes de retornar.
4. Si el flujo es asíncrono → Hangfire job. Nunca bloquear un thread de request esperando AFIP.

### Crear cambio SQL para deploy con Publish
1. Leer `.github/skills/migration-templates/SKILL.md` — templates listos para copiar.
2. Para estructura: editar objetos en `Molinos.Scato.Database/dbo/` (tablas, vistas, SPs, etc.).
3. Para datos de referencia: usar `Molinos.Scato.Database/Scripts/Post-Deployment/` con guardas idempotentes (`IF EXISTS` / `IF NOT EXISTS`).

---

## Restricciones críticas del stack

### EF5 — Entity Framework 5.0
- **No existen** `ToListAsync`, `SaveChangesAsync` ni ninguna API async de EF.
- Usar `AsNoTracking()` / `ListarNoTracking()` en todas las consultas read-only.
- Evitar N+1: usar `.Include()` o joins; nunca iterar y consultar dentro del loop.
- `DbContext` tiene scope de operación WCF — **nunca singleton**.
- No acceder a `ScatoDbContext` desde fuera de `Molinos.Scato.Repositorio`.

### WF4.5 Activities
- Solo `CodeActivity` (o `NativeActivity` si se necesitan bookmarks/children).
- **Sin** `async/await` en ninguna actividad.
- **Sin** constructor injection — resolver servicios exclusivamente con `context.GetExtension<T>()`.
- **Sin** acceso directo a `ScatoDbContext` desde actividades.
- Todas las excepciones en `Execute()` deben capturarse y mapearse a `Resultado.Errores`.
- Nunca modificar `InArgument`/`OutArgument` públicos — los `.xamlx` los referencian por nombre.

### AFIP
- Todas las llamadas AFIP van en `Servicios/Procesamiento/` — nunca desde controllers, actividades ni workflows directamente.
- Los proxies AFIP generados están excluidos de cobertura y no deben modificarse.
- Polling AFIP asíncrono corre vía Hangfire — nunca bloquear un thread de request.

### Mensajes de usuario y validación
- **Siempre** desde `Textos.*` (`Dominio/Recursos/Textos.resx`).
- **Nunca** strings literales en código de dominio, servicios, controllers ni actividades.
- `Textos.Designer.cs` es auto-generado — **no editar manualmente**.

### DI (Ninject 3.0)
- No usar Service Locator; inyectar por constructor en servicios y procesadores.
- Actividades WF son la excepción — usan `context.GetExtension<T>()` por limitación del framework.
- `DbContext` e `IRepositorio`: scope `InScope(ctx => OperationContext.Current)`.
- No usar singleton para `DbContext` ni `IRepositorio`.

### Build
- Usar **MSBuild de Visual Studio** (no `dotnet msbuild`) para compilar la solución completa:
  ```powershell
  & "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
      Molinos.Scato.sln /p:Configuration=Debug /nologo /m
  ```
- El repo tiene rutas con `%20` — los escapes en `.nuget/NuGet.targets` no deben revertirse.

---

## Instrucciones de capa (`.github/instructions/`)

Se inyectan automáticamente por glob. Los agentes deben leerlas explícitamente si necesitan reglas de esa capa:

| Archivo | Aplica a |
|---|---|
| `dominio.instructions.md` | `Dominio/Entidades/*.cs` |
| `command-processor.instructions.md` | `Dominio/Comandos/**/*.cs` |
| `dominio-validations.instructions.md` | `Dominio/Validations/*.cs` |
| `dominio-recursos.instructions.md` | `Dominio/Recursos/**` |
| `repositorio.instructions.md` | `Repositorio/RepositorioEF.cs` |
| `repositorio-consultas.instructions.md` | `Repositorio/ConsultasEF/*.cs` |
| `procesador.instructions.md` | `Servicios/Procesamiento/*.cs` |
| `conversiones-automapper.instructions.md` | `Servicios/Conversiones/**/*.cs` |
| `actividades.instructions.md` | `Actividades/**/*.cs` |
| `web-controllers.instructions.md` | `Web/Controllers/*.cs` |
| `webapi-controllers.instructions.md` | `WebPuertoApi/Controllers/*.cs` |
| `dependencias-di.instructions.md` | `Dependencias/*.cs` |
| `migraciones.instructions.md` | `Molinos.Scato.Database/**/*.sql` |
| `compilacion-entorno.instructions.md` | `*.sln`, `*.csproj` |
| `test.instructions.md` | `Test/**/*.cs` |

---

## Skills disponibles (`.github/skills/` — on-demand)

| Skill | Cuándo invocar |
|---|---|
| `afip-cpe-ctg` | Operaciones CPE/CTG, validación CUIT, manejo de errores AFIP |
| `entity-scaffold` | Scaffold completo de un caso de uso nuevo sobre una entidad |
| `domain-validations` | Implementar reglas de validación desde texto de negocio |
| `dotnet-best-practices` | Buenas prácticas .NET 4.5.2 / EF5 / WCF / Ninject |
| `dotnet-performance-fx472` | Análisis de performance strings, LINQ, EF5, async |
| `ef5-n-plus-one-review` | Detección de N+1 y refactors de consultas EF5 |
| `migration-templates` | Templates SQL idempotentes para `Molinos.Scato.Database` (Publish SSDT) |
| `wf-activity-refactor` | Refactor de `CodeActivity` con múltiples responsabilidades |
| `frontend-ko-signalr` | Patrones jQuery/Knockout/SignalR — Web (jQuery 1.11, KO 2.2.1) y WebMobile (jQuery 3.7.1, KO 3.5.1) |
| `azure-devops-cli` | CLI de Azure DevOps para pipelines y builds |
| `user-story` | Plantillas de historias de usuario y criterios de aceptación |
| `release-notes` | Clasificación de cambios, formato CHANGELOG, plantillas de release notes para PO |
| `copilot-project-setup` | Guía para crear/auditar la carpeta `.github/` |

---

## Glosario de dominio

| Término | Significado |
|---|---|
| `Recorrido` | Viaje logístico de transporte de granos |
| `CTG` | Constancia de Transporte de Granos (AFIP) |
| `CPE` | Carta de Porte Electrónica (AFIP) |
| `CartaPorte` | Documento de transporte regulado por AFIP |
| `AltaCTG` / `BajaCTG` | Crear / cerrar un CTG ante AFIP |
| `Calado` | Muestreo de calidad físico del grano |
| `Balanza` | Báscula de pesaje |
| `PesadaBruto` / `PesadaTara` | Pesadas de entrada y salida del camión |
| `Calle` | Muelle / bahía de carga en una instalación |
| `PuntoDeCarga` | Punto físico de carga |
| `Almacen` | Silo o depósito de granos |
| `Centro` | Planta / establecimiento logístico |
| `Transportista` | Empresa transportista |
| `Chofer` | Conductor del camión |
| `Fason` | Operación de maquila / proceso a terceros |
| `Coordinador` | Rol operativo de coordinación en planta |
| `CUIT` / `CUIL` | Identificador fiscal argentino |

---

## Build & Test

**Build**: Open `Molinos.Scato.sln` in Visual Studio 2017+ or run MSBuild:
```
msbuild Molinos.Scato.sln /p:Configuration=Debug
```

**CI Build** (Jenkins): Uses `Molinos.Scato.Build/build.proj` with targets:
```
msbuild build.proj /t:Build
msbuild build.proj /t:Testing
msbuild build.proj /t:CI
```

**Tests**: NUnit 2.6.3 + OpenCover coverage. Test project: `Molinos.Scato.Test/`.
- `Controllers/`, `ControllersMobile/` — Web controller tests
- `Actividades/` — Workflow activity tests
- `Servicios/` — Service layer tests
- `Procesamiento/` — Business logic tests
- `Mock/` — Mock implementations; `FactoryContext.cs` (raíz de `Molinos.Scato.Test/`) — test fixture factory

**Build configurations**: `Debug`, `DebugLocal`, `Jenkins`, `Release` (each with `AnyCPU` or `x86` platform).

## Architecture Patterns

- **Layered N-tier**: Presentation → Services → Repository → Domain
- **Repository pattern**: Use `IRepositorio<T>`, `IComando`, `IConsulta`, `IConsultaPaginada` — never call `ScatoDbContext` directly from outside `Molinos.Scato.Repositorio`
- **DI**: Ninject 3.0 — binding modules live in each project's `App_Start/`
- **Workflow engine**: Windows Workflow Foundation 4.5; operations are expressed as `.xaml` activities hosted in `.xamlx` state machines
- **Async jobs**: Hangfire for long-running background tasks (AFIP polling, SAP sync)
- **CQRS-lite**: Commands (`IComando`) mutate data; queries (`IConsulta`, `IConsultaPaginada`) are read-only

## Naming Conventions

- **Namespace root**: `Molinos.Scato.<Module>` (e.g., `Molinos.Scato.Web.Controllers`)
- **Entities**: PascalCase singular (e.g., `Recorrido`, `Vehiculo`, `Chofer`)
- **DTOs**: Entity name + `Dto` suffix
- **Interfaces**: `I` prefix (e.g., `IRepositorio`, `IServicioSap`)
- **Workflow activities**: Verb + noun (e.g., `AltaCTG`, `CargarCartaPorte`, `AnalisisDeCalidad`)
- **DB tables**: EF convention for pluralization is **disabled** — table names match entity class names

## Base classes and interfaces — quick reference

### Processor layer (`Molinos.Scato.Servicios/Procesamiento/`)

| Base class | Use when |
|---|---|
| `ProcesadorCrear<TComando>` | Creating a new entity |
| `ProcesadorModificar<TComando>` | Updating an existing entity |
| `ProcesadorEliminar<TComando>` | Deleting an entity |
| `ProcesadorComando<TComando>` | Any other business operation |

All processors:
- Override `Validar(comando, resultado)` — add errors with `resultado.Error("Campo", Textos.Mensaje)`.
- Override `Ejecutar(comando)` — persist via injected `Repositorio`.
- Always return `Resultado` or `Resultado<T>`, never `null`, never throw for business errors.

### Repository layer (`Molinos.Scato.Repositorio/`)

| Interface | Purpose |
|---|---|
| `IRepositorio<T>` | CRUD + `Existe`, `Listar`, `ListarNoTracking`, `ObtenerPorId` |
| `IConsulta<TParam, TResult>` | Read-only single-result query |
| `IConsultaEscalar<TParam, TResult>` | Scalar read-only query |
| `IConsultaPaginada<TParam, TResult>` | Paginated read-only query |
| `IComando<TParam>` | Low-level data mutation (non-processor) |

### Result objects (`Molinos.Scato.Dominio/Comandos/`)

| Type | Use |
|---|---|
| `Resultado` | Void operations — check `resultado.EsValido` |
| `Resultado<T>` | Returns typed data |
| `ResultadoCrear` | Creation — carries new entity `Id` |

### Command naming convention

- Verb + noun matching processor: `CrearAlmacen`, `ModificarChofer`, `EliminarCalle`.
- Base: `Comando` (generic), `ComandoImpresion`, `ComandoSincronizar` for specializations.

## Key Domain Concepts

| Term | Meaning |
|------|---------|
| `Recorrido` | A logistics journey record (grain transport trip) |
| `CTG` / `CPE` | *Constancia de Portación Electrónica* — AFIP digital cargo manifest |
| `AltaCTG` / `BajaCTG` | Create / close a CTG with AFIP |
| `Calado` | Quality sampling (physical grain probe) |
| `Balanza` | Weighbridge scale |
| `Calle` | Docking lane at a facility |
| `PuntoDeCarga` | Loading point |
| `Almacén` | Storage silo/warehouse |
| `Transportista` | Carrier company |
| `Chofer` | Driver |
| `Fason` | Toll-processing (maquiladora) operation |

## AFIP Integration

See [README.md](README.md) for the full endpoint table mapping CPE web service operations (`autorizarCPEAutomotor`, `anularCPE`, `confirmarArriboCPE`, etc.) to controllers and workflows.

Key services in `Molinos.Scato.Servicios/`:
- AFIP CPE proxy calls are wrapped in service classes; never call AFIP endpoints directly from controllers or activities
- Generated AFIP/SAP web-service proxies are **excluded** from code coverage filters

## Database & Migrations

- **ORM**: Entity Framework 5.0 — entities auto-mapped from `Molinos.Scato.Dominio.Entidades` namespace
- **Connection string key**: `ScatoDb`
- **Schema source of truth**: `Molinos.Scato.Database/Molinos.Scato.Database.sqlproj` (SQL Server Data Tools)
- **Deployment mode**: SSDT **Publish** (`*.publish.xml` profiles)
- **Post-deploy data**: `Molinos.Scato.Database/Scripts/Post-Deployment/Datos Base.sql` (y scripts referenciados por el `.sqlproj`)
- **Legacy**: `Molinos.Scato.Migrations/` queda como referencia histórica, no como flujo principal de despliegue

## Configuration & Environments

- `Web.config` / `App.config` with per-environment XDT transforms (`Web.Debug.config`, `Web.Jenkins.config`, etc.)
- Deployment environments: `VICSCATOPROD` (prod), `VICSCATOQA`, `VICSCATOUAT`, `GSLOSCATO*`, `ASVWUSCATO00`
- Deploy parameters: `*.DeployParameters.xml` per site in each web project
- Logging: Log4net (`log4net.config`), ELMAH, Application Insights, NewRelic

## Technologies At a Glance

| Layer | Tech |
|-------|------|
| Runtime | .NET Framework 4.5.2 |
| Web | ASP.NET MVC 4, Web API 2, SignalR 2.4.1 |
| Workflow | Windows Workflow Foundation 4.5 |
| DI | Ninject 3.0 |
| ORM | Entity Framework 5.0 |
| DB | SQL Server |
| Frontend | jQuery, Bootstrap 2.3.2, Knockout 2.2.1 |
| Serialization | Newtonsoft.Json 11.0.2 |
| HTTP Client | RestSharp 106.15.0 |
| PDF | iTextSharp 5.5.13 |
| Excel | ExcelDataReader, NPOI |
| Reporting | SSRS (RDL files) |
| Background Jobs | Hangfire |
| Testing | NUnit 2.6.3, OpenCover |
| Monitoring | App Insights, NewRelic, Log4net, ELMAH |
