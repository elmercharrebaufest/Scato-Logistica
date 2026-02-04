# Molinos Scato - Logistics System AI Instructions

## Architecture Overview

This is a .NET Framework 4.5.2-4.7.2 logistics system for grain and cargo management using **Windows Workflow Foundation (WF)** as the core orchestration engine. The system integrates with AFIP (Argentina's tax authority) for electronic waybills (CTG/CPE) and SAP for ERP operations.

### Key Components

- **Molinos.Scato.Workflow** - WCF-hosted workflow runtime (XAML workflows, `.xamlx` files)
- **Molinos.Scato.Actividades** - Custom workflow activities (XAML-based, not code activities)
- **Molinos.Scato.Dominio** - Domain entities (~300 classes in `Entidades/`)
- **Molinos.Scato.Repositorio** - EF6 repository pattern with command/query separation
- **Molinos.Scato.Web** - Main ASP.NET MVC web application
- **Molinos.Scato.WebMobile** / **WebOperaciones** / **WebPuerto** / **WebPuertoApi** - Specialized portals
- **Molinos.Scato.Servicios** - Business services and external integrations

## Data Access Pattern

This codebase uses **Command-Query Separation** with Entity Framework:

```csharp
// Commands execute actions, return Resultado
public interface IComando<TResultado> {
    TResultado Ejecutar(DbContext contexto);
}

// Queries retrieve data, return List<T>
public interface IConsulta<TEntidad> {
    List<TEntidad> Ejecutar(DbContext contexto);
}
```

**Usage:**
- Commands/Queries are in `Molinos.Scato.Dominio/Comandos/` and `Consultas/`
- Execute via `IServicioComandos.Ejecutar()` or `IServicioRepositorio.Ejecutar()`
- Repository methods for simple CRUD: `Obtener<T>(id)`, `Listar<T>(filter)`, `Guardar(entity)`

## Workflow Architecture

### XAML Workflows
- Workflows are **declarative XAML files** (`.xaml`, `.xamlx`) in `Molinos.Scato.Actividades/`
- Hosted as WCF services in `Molinos.Scato.Workflow` with `WorkflowServiceHost`
- Persistent workflows use `SqlWorkflowInstanceStore` configured in Web.config
- Custom activities are XAML-based, not code activities

### Workflow Integration
```csharp
// Starting workflows from controllers
var workflowPath = $"{urlBaseWorkflow}{workflow}.xamlx";
var inputs = new Dictionary<string, object> {
    {"CartaPorteId", orden.Id},
    {"UsuarioId", datosUsuario.UsuarioId}
};
servicioWorkflows.Invocar(workflowPath, inputs);
```

### Custom Behaviors
- **ServiciosScatoBehavior** - Injects services into workflow extensions (cannot use Ninject in ServiceHost)
- **PromotePropertiesBehavior** - Property promotion for workflow tracking (custom properties like Patente, MaterialId)
- Extensions added via `host.WorkflowExtensions.Add(() => service)`

## AFIP Integration (Electronic Waybills)

### CTG/CPE Operations
The system heavily integrates with AFIP for Cartas de Porte Electrónicas:

```csharp
// Query CPE from AFIP
var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital { 
    NroCtg = numeroCtg, 
    CentroId = centroId 
}) as ResultadoCartaPorteElectronica;

// Create new CPE
servicioComandos.Ejecutar(new AutorizarCpe { 
    Dto = cartaPorteDto, 
    VehiculoId = vehiculoId 
});
```

### AFIP Service Endpoints
- **autorizarCPEAutomotor/Ferroviaria** - Create truck/train waybills
- **consultarCPEAutomotor/Ferroviaria** - Query existing waybills
- **confirmarArriboCPE** - Confirm arrival
- **confirmacionDefinitivaCPE** - Final confirmation
- **anularCPE** - Cancel waybill
- Refer to [README.md](README.md#servicios) AFIP table for complete endpoint mapping

## Naming Conventions

- **Entities**: Spanish names (e.g., `CartaPorte`, `Recorrido`, `Vehiculo`)
- **Commands/Queries**: Spanish descriptive names (e.g., `ConsultarCPDigital`, `AutorizarCpe`)
- **Controllers**: Suffix with `Controller` (e.g., `CargarCartaPorteController`)
- **Services**: Interface prefix `I` (e.g., `IServicioRepositorio`)
- **DTOs**: Suffix with `Dto` (e.g., `CartaPorteDto`)

## Database Context

- **ScatoDbContext** - Single EF6 DbContext
- Connection string named `"ScatoDb"` in Web.config
- No pluralized table names (`PluralizingTableNameConvention` removed)
- Cascade delete disabled globally
- Entities auto-mapped from `Molinos.Scato.Dominio.Entidades` namespace

## Testing Workflows

Use `WorkflowInvokerTest` from `Microsoft.Activities.UnitTesting` for testing activities:

```csharp
var target = new MyActivity();
var host = WorkflowInvokerTest.Create(target);
host.Extensions.Add(mockRepositorio);
host.InArguments.PropertyName = value;
var result = host.TestActivity();
```

## Build & Database Migrations

- **RoundHouse** for database migrations (`Molinos.Scato.Migrations/`)
- Jenkins build configuration (`.Jenkins` configs in multiple projects)
- Multiple deployment environments: QA, UAT, PRD

## External Integrations

1. **SAP** - ERP integration via `ZSDWS_SCATO` service (movements, stock, invoicing)
2. **AFIP** - Tax authority CPE/CTG services (`AfipCPDigitalService`)
3. **Monsanto** - Waybill management (`WaybillManagementPODv2`)
4. **Compliance** - Internal compliance service
5. **Mercado Pago** - Payment processing

## Common Patterns

### Service Locator
```csharp
ServiceProvider.Current.Get<IServicioRepositorio>()
```

### Activity Results
Activities return `Resultado` or derived types with error handling:
```csharp
if (resultado.HayErrores) {
    // resultado.Errores is Dictionary<string, string>
}
```

### Authorization
Use `[Autorizacion(PermisosScato.PermissionName)]` attribute on controllers/actions.

## Key Files to Reference

- [Web.config](Molinos.Scato.Web/Web.config) - Main app settings, connection strings
- [README.md](README.md) - AFIP service endpoint documentation
- [ScatoDbContext.cs](Molinos.Scato.Repositorio/ScatoDbContext.cs) - EF configuration
- [IRepositorio.cs](Molinos.Scato.Repositorio/IRepositorio.cs) - Repository interface

## Important Notes

- This is NOT .NET Core - use .NET Framework patterns
- Workflows are XAML-declarative, not imperative code
- Spanish business domain terminology throughout
- Heavy use of dependency injection via Ninject (except in workflow hosts)
- SignalR for real-time notifications (`ServicioHub`)
