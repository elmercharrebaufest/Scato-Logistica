# Molinos Scato Logistica

Plataforma de logistica agroindustrial para gestionar recorridos, balanza, calidad y cumplimiento AFIP (CPE/CTG) en operaciones de planta, puerto y almacenamiento.

## Getting Started

### Requisitos
- Visual Studio 2017+ o MSBuild compatible con .NET Framework 4.5.2
- SQL Server accesible para `ScatoDb`

### Solucion principal
- `Molinos.Scato.sln`

### Proyectos clave
- `Molinos.Scato.Web`: portal principal MVC4
- `Molinos.Scato.Servicios`: servicios WCF e integraciones
- `Molinos.Scato.Repositorio`: acceso a datos EF5
- `Molinos.Scato.Dominio`: entidades y reglas de negocio
- `Molinos.Scato.Actividades` + `Molinos.Scato.Workflow`: motor WF4.5

## Build and Test

```powershell
msbuild Molinos.Scato.sln /p:Configuration=Debug
```

Para build estilo CI (desde `Molinos.Scato.Build`):

```powershell
msbuild build.proj /t:Build
msbuild build.proj /t:Testing
```

## Azure DevOps CLI compartido

La configuracion base del equipo queda en `AiEnablement\azure-devops.defaults.psd1` y el bootstrap en `AiEnablement\Setup-AzureDevOpsCli.ps1`.

```powershell
.\AiEnablement\Setup-AzureDevOpsCli.ps1
```

Si queres que tambien inicie sesion en Azure CLI en ese mismo paso:

```powershell
.\AiEnablement\Setup-AzureDevOpsCli.ps1 -Login
```


## Contribute

- Respetar arquitectura por capas: Web -> Servicios -> Repositorio -> Dominio.
- No acceder a `ScatoDbContext` fuera de Repositorio.
- Mantener patron comando/procesador y consultas EF existentes.
- Para cambios de reglas de negocio, actualizar documentacion asociada en el mismo PR.

## Servicios

### AFIP

| endpoint | Descripcion | Logistica | Operaciones | Workflows | Jobs | ABMs
| -- | -- | -- | -- | -- | -- | -- |
| autorizarCPEAutomotor | Crear carta de porte para camiones de granos | AltaCTGController | - | Salida de Mercaderia | - | - | -
| autorizarCPEFerroviaria | Crear carta de porte para trenes de granos | AltaCTGController | - | Salida de Mercaderia | - | - | 
| autorizarCPEAutomotorDG | Crear carta de porte para camiones de no granos | AltaCTGDGController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| anularCPE | Anular una carta de porte | IngresarOrdenCargaFasController, IngresarOrdenCargaInternaController, IngresarOrdenCargaInternaFasonController, CamionDemoradoController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| consultarUltNroOrden | Obtener el número de orden de la última carta de porte creada en una sucursal | AltaCTGController, AltaCTGDGController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas, Salida de Mercaderia | - | - | 
| consultarPlantasDG | Obtener todas las plantas derivado granario de un CUIT | ConsultasController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| consultarDomiciliosPorCUIT | Obtener todas los domicilios de un CUIT | ConsultasController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| consultarCPEPPendientesDeResolucion | Obtener CPE que hayan vencido | IngresarOrdenCargaFasController, IngresarOrdenCargaInternaController, IngresarOrdenCargaInternaFasonController, CamionDemoradoController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| consultarCPEAutomotorDG | Obtener datos de una CPEDG |   ConsultasController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas, Recepcion Cliente Fason | - | - | 
| descargadoDestinoCPE | Actualizar estado de CPE a descargado destino | IngresarOrdenCargaFasController, IngresarOrdenCargaInternaController, IngresarOrdenCargaInternaFasonController, CamionDemoradoController | - | Egreso Cliente Fason, Egreso Cliente Fason Sin Flete, Egreso Material No Productivo, Egreso Venta Fas | - | - | 
| consultarCPEAutomotor | Obtener datos de una CPE de camion | CargarCartaPorte, IngresarCartaPorteRedespachoImportaciones, IngresarCartaPorteRedespacho | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho | Job Actualizar Cachear CPE AFIP | Monitor CPE Cacheada | 
| consultarCPEFerroviaria | Obtener datos de una CPE de tren | CargarCartaPorte, IngresarCartaPorteRedespachoImportaciones, IngresarCartaPorteRedespacho | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho | Job Actualizar Cachear CPE AFIP | Monitor CPE Cacheada | 
| confirmarArriboCPE | Actualizar estado de CPE a Confirmacion de Arribo | BajaCTGController | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho, Recepcion Cliente Fason | - | Panel de control Baja de CTG | 
| confirmacionDefinitivaCPEAutomotor | Actualizar estado de CPE de camiones a Confirmacion Definitiva | BajaCTGDefinitivoController | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho | - | Panel de control Baja de CTG | 
| confirmacionDefinitivaCPEFerroviaria | Actualizar estado de CPE de trenes a Confirmacion Definitiva | BajaCTGDefinitivoController | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho | - | Panel de control Baja de CTG | 
| confirmacionDefinitivaCPEAutomotorDG | Actualizar estado de CPE de camiones no granos a Confirmacion Definitiva | BajaCTGDGController | - | Recepcion Cliente Fason | - | - | 
| consultarLocalidadesPorProvincia | Obtener localidades de una provincia |  | - | Salida de Mercaderia | - | - | 
| consultaCPEFerroviariaPorNroOperativo | Obtener todas las CPE de vagones asociados a un mismo número de operativo |   CargarCartaPorte, IngresarCartaPorteRedespachoImportaciones, IngresarCartaPorteRedespacho | - | Ingreso por Compra de Granos, Ingreso por Importaciones, Ingreso por Redespacho | - | - | 
| consultarCPEPorDestino | Obtener CPE activos que van hacia MOA |   - | - | - | Job Actualizar Cachear CPE AFIP | - | 
| dummy | Consultar estado de web service de AFIP | PanelServiciosWebController | - | - | Job Estado de Servicios | - | 
