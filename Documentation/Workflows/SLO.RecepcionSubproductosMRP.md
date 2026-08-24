# Workflow: SLO_RecepcionSubproductosMRP

> **Archivo:** `SLO.RecepcionSubproductosMRP.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 25  
> **Variables de scope:** 21  

## Descripcion de negocio

_TODO: Agregar descripcion del proceso de negocio._

## Variables de scope

| Nombre | Tipo | Valor por defecto |
|--------|------|-------------------|
| `autorizarTiempo` | `Boolean` | -- |
| `balanzaId` | `Int32` | -- |
| `centroId` | `Int32` | -- |
| `debeRepesar` | `Boolean` | -- |
| `excedePesoMaximo` | `Boolean` | -- |
| `fechaInicio` | `DateTime` | -- |
| `fechaPesoBruto` | `DateTime` | -- |
| `fechaPesoTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `nombreUsuario` | `String` | nombreTest |
| `observacion` | `String` | -- |
| `orden` | `RemitoDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaHabilitado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |
| `vehiculoRechazado` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[IngresoRemito]
    A1[VerificacionTransportistaHabilitado]
    A2[Visteo]
    A3[AsignacionTarjetaDeAcceso]
    A4[EnPlayaExterna]
    A5[ImprimeReciboMunicipal]
    A6[EnTransito]
    A7[AutorizarTiempoEnTransito]
    A8[PuestoComando]
    A9[PesadaBruto]
    A10[RefrescarRemito]
    A11[ImpresionAsignacionDeRuta]
    A12[ControlPesoMaximo]
    A13[BalanzaACero]
    A14[ConfirmacionDeCargaDescarga]
    A15[PesadaTara]
    A16[VerificacionCamionRechazado]
    A17[BalanzaACero]
    A18[ImpresionFormulario239]
    A19[GuardarFechaEgreso]
    A20[SalidaDeCentro]
    A21[ImpresionTicketPesada]
    A22[ServicioSapMov305]
    A23[AutorizarTransportistaInhabilitado]
    D24{"transportistaHabilitado"}
    D25{"vehiculoEnCondiciones"}
    D26{"autorizarTiempo"}
    D27{"excedePesoMaximo"}
    D28{"debeRepesar"}
    D29{"vehiculoRechazado"}
    D30{"transportistaAutorizado"}
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
    A12 --> A17
    A17 --> A14
    A14 --> A15
    A15 --> A16
    A16 --> A17
    A17 --> A18
    A18 --> A19
    A19 --> A20
    A20 --> A21
    A21 --> A22
    A22 --> A23
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `IngresoRemito` | hndInstance, tipoDocumentoIngreso, centroId, orden, fechaInicio |
| 2 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 3 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 4 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 5 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 6 | `ImprimeReciboMunicipal` | workflowId, materialId, puestoDeTrabajoId, centroId, codigoDeImpresion |
| 7 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 8 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 9 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 10 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 11 | `RefrescarRemito` | workflowId, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 12 | `ImpresionAsignacionDeRuta` | Calado, CentroId, CodigoDeImpresion, Patente, MaterialId |
| 13 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, tipoPesoMaximo, workflowId |
| 14 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 15 | `ConfirmacionDeCargaDescarga` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 16 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 17 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 18 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 19 | `ImpresionFormulario239` | Observaciones, PuestoDeTrabajoId, MaterialId, WorkflowId, CodigoDeImpresion |
| 20 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 21 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 22 | `ImpresionTicketPesada` | Observaciones, PuestoDeTrabajoId, MaterialId, NumeroDocumento, WorkflowId |
| 23 | `ServicioSapMov305` | hndInstance, nombreUsuario, centroId, tipoComercialId, ejercicio |
| 24 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |

**Decisiones de flujo:**

- **Transportista habilitado?**: `[transportistaHabilitado]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Se Repesa Vehiculo?**: `[debeRepesar]`
- **Se Rechaza Vehículo?**: `[vehiculoRechazado]`
- **Autoriza?**: `[transportistaAutorizado]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `ConfirmacionDeCargaDescarga`
- `ControlPesoMaximo`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionAsignacionDeRuta`
- `ImpresionAsignacionDeRuta.Calidad`
- `ImpresionFormulario239`
- `ImpresionFormulario239.Representante`
- `ImpresionTicketPesada`
- `ImprimeReciboMunicipal`
- `IngresoRemito`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarRemito`
- `SalidaDeCentro`
- `ServicioSapMov305`
- `VerificacionCamionRechazado`
- `VerificacionTransportistaHabilitado`
- `Visteo`

## Dependencias de dominio

- `Molinos.Scato.Dominio`

---
_Documentacion generada automaticamente por `AiEnablement/Generate-WfDocs.ps1`._