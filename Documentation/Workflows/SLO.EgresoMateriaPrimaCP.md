# Workflow: SLO.EgresoMateriaPrimaCP

> **Archivo:** `SLO.EgresoMateriaPrimaCP.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 28  
> **Variables de scope:** 26  

## Descripcion de negocio

_TODO: Agregar descripcion del proceso de negocio._

## Variables de scope

| Nombre | Tipo | Valor por defecto |
|--------|------|-------------------|
| `autorizarTiempo` | `Boolean` | -- |
| `balanzaId` | `Int32` | -- |
| `centroId` | `Int32` | -- |
| `excedePesoMaximo` | `Boolean` | -- |
| `existePedidoTraslado` | `Boolean` | -- |
| `existeTransportista` | `Boolean` | -- |
| `fechaBruto` | `DateTime` | -- |
| `fechaInicio` | `DateTime` | -- |
| `fechaTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `loteNro` | `String` | -- |
| `nombreUsuario` | `String` | NombreTest |
| `observacion` | `String` | -- |
| `orden` | `CartaPorteDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `rechazado` | `Boolean` | -- |
| `repesar` | `Boolean` | -- |
| `salidaVerificada` | `Boolean` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaHabilitado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculo` | `VehiculoDto` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[RefrescarCartaDePorte]
    A1[PesadaBruto]
    A2[VerificacionCamionRechazado]
    A3[BalanzaACero]
    A4[ImpresionFormulario239]
    A5[GuardarFechaEgreso]
    A6[SalidaDeCentro]
    A7[ControlPesoMaximo]
    A8[BalanzaACero]
    A9[EnEsperaIndianapolis]
    A10[AltaCTG]
    A11[GuardarFechaEgreso]
    A12[ServicioSapIngresosEgresosFazones]
    A13[VerificacionCot]
    A14[ImpresionTicketPesada]
    A15[ImpresionGenerica]
    A16[SalidaDeCentro]
    A17[Visteo]
    A18[AsignacionTarjetaDeAcceso]
    A19[EnPlayaExterna]
    A20[ImprimeReciboMunicipal]
    A21[EnTransito]
    A22[AutorizarTiempoEnTransito]
    A23[PuestoComando]
    A24[PesadaTara]
    A25[BalanzaACero]
    A26[AutorizarTransportistaInhabilitado]
    A27[VerificacionTransportistaHabilitado]
    A28[IngresoDeTransportista]
    A29[VerificacionTransportistaExiste]
    A30[CargarCartaPorteFason]
    D31{"rechazado"}
    D32{"excedePesoMaximo"}
    D33{"repesar"}
    D34{"transportistaAutorizado"}
    D35{"vehiculoEnCondiciones"}
    D36{"autorizarTiempo"}
    D37{"transportistaHabilitado"}
    D38{"existeTransportista"}
    A0 --> A1
    A1 --> A2
    A2 --> A25
    A25 --> A4
    A4 --> A11
    A11 --> A16
    A16 --> A7
    A7 --> A25
    A25 --> A9
    A9 --> A10
    A10 --> A11
    A11 --> A12
    A12 --> A13
    A13 --> A14
    A14 --> A15
    A15 --> A16
    A16 --> A17
    A17 --> A18
    A18 --> A19
    A19 --> A20
    A20 --> A21
    A21 --> A22
    A22 --> A23
    A23 --> A24
    A24 --> A25
    A25 --> A26
    A26 --> A27
    A27 --> A28
    A28 --> A29
    A29 --> A30
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `RefrescarCartaDePorte` | workflowId, vehiculo, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 2 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 3 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 4 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 5 | `ImpresionFormulario239` | Observaciones, PuestoDeTrabajoId, MaterialId, WorkflowId, CodigoDeImpresion |
| 6 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 7 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 8 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 9 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 10 | `EnEsperaIndianapolis` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 11 | `AltaCTG` | hndInstance, centroId, orden, vehiculo, workflowId |
| 12 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 13 | `ServicioSapIngresosEgresosFazones` | nombreUsuario, instanceId, vehiculoId, TipoMovimiento, MaterialId |
| 14 | `VerificacionCot` | hndInstance, puestoDeTrabajoId, instanceId, nombreUsuario |
| 15 | `ImpresionTicketPesada` | Observaciones, PuestoDeTrabajoId, MaterialId, NumeroDocumento, WorkflowId |
| 16 | `ImpresionGenerica` | PesoBrutoOrigen, ProvinciaDestino, TarifaTonelada, Corredor, PesoNeto |
| 17 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 18 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 19 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 20 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 21 | `ImprimeReciboMunicipal` | workflowId, materialId, puestoDeTrabajoId, centroId, codigoDeImpresion |
| 22 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 23 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 24 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 25 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 26 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 27 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |
| 28 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 29 | `IngresoDeTransportista` | workflowId, CartaPorte, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 30 | `VerificacionTransportistaExiste` | WorkflowId, Result, PuestoDeTrabajoId, TransportistaId |
| 31 | `CargarCartaPorteFason` | hndInstance, tipoDocumentoIngreso, centroId, orden, vehiculo |

**Decisiones de flujo:**

- **Camión Rechazado?**: `[rechazado]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Repesa vehículo?**: `[repesar]`
- **Autoriza?**: `[transportistaAutorizado]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **Autoriza Tiempo?**: `[autorizarTiempo]`
- **Transportista Habilitado?**: `[transportistaHabilitado]`
- **Existe Transportista?**: `[existeTransportista]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AltaCTG`
- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `CargarCartaPorteFason`
- `ControlPesoMaximo`
- `EnEsperaIndianapolis`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionFormulario239`
- `ImpresionFormulario239.Representante`
- `ImpresionGenerica`
- `ImpresionTicketPesada`
- `ImprimeReciboMunicipal`
- `IngresoDeTransportista`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarCartaDePorte`
- `SalidaDeCentro`
- `ServicioSapIngresosEgresosFazones`
- `VerificacionCamionRechazado`
- `VerificacionCot`
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