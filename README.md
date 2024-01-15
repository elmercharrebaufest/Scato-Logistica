# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

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
