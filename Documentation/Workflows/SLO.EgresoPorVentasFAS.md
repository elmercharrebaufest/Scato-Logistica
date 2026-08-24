# Workflow: SLO_EgresoPorVentasFAS

> **Archivo:** `SLO.EgresoPorVentasFAS.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 35  
> **Variables de scope:** 26  

## Descripcion de negocio

_TODO: Agregar descripcion del proceso de negocio._

## Variables de scope

| Nombre | Tipo | Valor por defecto |
|--------|------|-------------------|
| `autorizarTiempo` | `Boolean` | -- |
| `balanzaId` | `Int32` | -- |
| `camionRechazado` | `Boolean` | -- |
| `centroId` | `Int32` | -- |
| `codigoRespuesta` | `Int32` | -- |
| `excedePesoMaximo` | `Boolean` | -- |
| `existeTransportista` | `Boolean` | -- |
| `fechaPesoBruto` | `DateTime` | -- |
| `fechaPesoTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `mensajeRespuesta` | `String` | -- |
| `nombreUsuario` | `String` | Test |
| `observaciones` | `String` | -- |
| `orden` | `OrdenCargaFasDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `pideServicioCompliance` | `Boolean` | -- |
| `repesar` | `Boolean` | -- |
| `salidaVerificada` | `Boolean` | -- |
| `tipoCodumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaValido` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |
| `vehiculoRechazado` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[IngresarOrdenCargaFas]
    A1[VerificacionTransportistaExiste]
    A2[VerificacionSalidaFlete]
    A3[VerificacionTransportistaHabilitado]
    A4[Visteo]
    A5[AsignacionTarjetaDeAcceso]
    A6[EnPlayaExterna]
    A7[ImprimeReciboMunicipal]
    A8[EnTransito]
    A9[AutorizarTiempoEnTransito]
    A10[PuestoComando]
    A11[PesadaTara]
    A12[RefrescarOrdenCargaFas]
    A13[ImpresionAsignacionDeRuta]
    A14[BalanzaACero]
    A15[ConfirmacionDeCargaDescarga]
    A16[PesadaBruto]
    A17[VerificacionCamionRechazado]
    A18[BalanzaACero]
    A19[ImpresionFormulario239]
    A20[GuardarFechaEgreso]
    A21[SalidaDeCentro]
    A22[ControlPesoMaximo]
    A23[BalanzaACero]
    A24[EnEsperaIndianapolis]
    A25[ControlDePesoEsperado]
    A26[BalanzaACero]
    A27[IngresoDeObservaciones]
    A28[ServicioSapPesaNeto]
    A29[VerificacionCot]
    A30[ImpresionTicketPesada]
    A31[AutorizarTransportistaInhabilitado]
    A32[IngresoDeTransportistaOrdenCargaFas]
    D33{"existeTransportista"}
    D34{"orden.ValidaCompliance"}
    D35{"salidaVerificada"}
    D36{"transportistaValido"}
    D37{"vehiculoEnCondiciones"}
    D38{"autorizarTiempo"}
    D39{"camionRechazado"}
    D40{"excedePesoMaximo"}
    D41{"repesar"}
    D42{"repesar"}
    D43{"transportistaAutorizado"}
    A0 --> A1
    A1 --> A2
    A2 --> A3
    A3 --> A4
    A4 --> A5
    A5 --> A6
    A6 --> A7
    A7 --> A8
    A8 --> A9
    A9 --> A10
    A10 --> A11
    A11 --> A12
    A12 --> A13
    A13 --> A26
    A26 --> A15
    A15 --> A16
    A16 --> A17
    A17 --> A26
    A26 --> A19
    A19 --> A20
    A20 --> A21
    A21 --> A22
    A22 --> A26
    A26 --> A24
    A24 --> A25
    A25 --> A26
    A26 --> A27
    A27 --> A28
    A28 --> A29
    A29 --> A30
    A30 --> A31
    A31 --> A32
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `IngresarOrdenCargaFas` | validaCompliance, tipoDocumentoIngreso, centroId, hdnInstance, orden |
| 2 | `VerificacionTransportistaExiste` | WorkflowId, Result, PuestoDeTrabajoId, TransportistaId |
| 3 | `VerificacionSalidaFlete` | hndInstance, choferId, centroId, transportistaId, patente |
| 4 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 5 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 6 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 7 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 8 | `ImprimeReciboMunicipal` | centroId, codigoDeImpresion, materialId, workflowId, puestoDeTrabajoId |
| 9 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 10 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 11 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 12 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 13 | `RefrescarOrdenCargaFas` | workflowId, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 14 | `ImpresionAsignacionDeRuta` | Calado, CentroId, WorkflowId, Patente, MaterialId |
| 15 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 16 | `ConfirmacionDeCargaDescarga` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 17 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 18 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 19 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 20 | `ImpresionFormulario239` | Observaciones, PuestoDeTrabajoId, MaterialId, WorkflowId, NumeroDeFormulario |
| 21 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 22 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 23 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 24 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 25 | `EnEsperaIndianapolis` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 26 | `ControlDePesoEsperado` | hndInstance, repesar, tipoComercialId, workflowId, pesoNeto |
| 27 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 28 | `IngresoDeObservaciones` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 29 | `ServicioSapPesaNeto` | hndInstance, CentroId, TipoComercialId, NumeroDocumento, MaterialId |
| 30 | `VerificacionCot` | hndInstance, puestoDeTrabajoId, instanceId, nombreUsuario |
| 31 | `ImpresionTicketPesada` | PuestoDeTrabajoId, MaterialId, NumeroDocumento, WorkflowId, TipoDocumento |
| 32 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |
| 33 | `IngresoDeTransportistaOrdenCargaFas` | workflowId, hndInstance, cargaFas, puestoDeTrabajoId, nombreUsuario |

**Decisiones de flujo:**

- **Existe el Transportista?**: `[existeTransportista]`
- **Valida Compliance?**: `[orden.ValidaCompliance]`
- **Salida Flete Habilitada?**: `[salidaVerificada]`
- **Transportista Habilitado?**: `[transportistaValido]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **Camión Rechazado?**: `[camionRechazado]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Se Repesa el Vehículo?**: `[repesar]`
- **Se Repesa el Vehículo?**: `[repesar]`
- **Autoriza?**: `[transportistaAutorizado]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `ConfirmacionDeCargaDescarga`
- `ControlDePesoEsperado`
- `ControlPesoMaximo`
- `EnEsperaIndianapolis`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionAsignacionDeRuta`
- `ImpresionAsignacionDeRuta.Calidad`
- `ImpresionAsignacionDeRuta.CodigoDeImpresion`
- `ImpresionFormulario239`
- `ImpresionFormulario239.CodigoDeImpresion`
- `ImpresionTicketPesada`
- `ImpresionTicketPesada.CodigoDeImpresion`
- `ImpresionTicketPesada.Observaciones`
- `ImprimeReciboMunicipal`
- `IngresarOrdenCargaFas`
- `IngresoDeObservaciones`
- `IngresoDeTransportistaOrdenCargaFas`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarOrdenCargaFas`
- `SalidaDeCentro`
- `ServicioSapPesaNeto`
- `VerificacionCamionRechazado`
- `VerificacionCot`
- `VerificacionSalidaFlete`
- `VerificacionTransportistaExiste`
- `VerificacionTransportistaHabilitado`
- `Visteo`

## Dependencias de dominio

- `Molinos.Scato.Actividades`
- `Molinos.Scato.Dependencias`
- `Molinos.Scato.Dominio`
- `Molinos.Scato.Servicios`
- `Molinos.Scato.Workflow`

---
_Documentacion generada automaticamente por `AiEnablement/Generate-WfDocs.ps1`._