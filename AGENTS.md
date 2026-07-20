# Molinos Scato Logística — Agent Instructions

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
| `Molinos.Scato.Migrations/` | RoundHouse database migration scripts |
| `Molinos.Scato.Database/` | SQL Server Data Tools project (`.sqlproj`) |
| `Molinos.Scato.Reportes/` | SSRS reports (`.rptproj`, `.rdl`) |
| `Molinos.Scato.Test/` | NUnit tests |
| `Molinos.Scato.Build/` | MSBuild orchestration (`build.proj`, `build.targets`) |

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
- `Mock/` — Mock implementations; `FactoryContext.cs` — test fixture factory

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
- **Migrations tool**: RoundHouse; scripts live in `Molinos.Scato.Migrations/`
  - Naming: `R.XX.YY.ZZ-PATTERN.dbo.TableName[.ENV].sql`
  - `10 Indexes/`, `11 RunAfterOtherAnyTimeScripts/` for post-deploy scripts
- **Schema project**: `Molinos.Scato.Database.sqlproj` (SQL Server Data Tools)

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
