# Workflow: SLO_EgresoClienteFasonSinFlete

> **Archivo:** `SLO.EgresoClienteFasonSinFlete.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 31  
> **Variables de scope:** 24  

## Descripcion de negocio

_TODO: Agregar descripcion del proceso de negocio._

## Variables de scope

| Nombre | Tipo | Valor por defecto |
|--------|------|-------------------|
| `autorizarTiempo` | `Boolean` | -- |
| `balanzaId` | `Int32` | -- |
| `camionRechazado` | `Boolean` | -- |
| `centroId` | `Int32` | -- |
| `debeRepesar` | `Boolean` | -- |
| `excedePesoMaximo` | `Boolean` | -- |
| `existeTransportista` | `Boolean` | -- |
| `fechaInicio` | `DateTime` | -- |
| `fechaPesoBruto` | `DateTime` | -- |
| `fechaPesoTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `nombreUsuario` | `String` | nombreTest |
| `numeroDocumentoIngreso` | `String` | -- |
| `observacion` | `String` | -- |
| `orden` | `OrdenCargaInternaFasonDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `salidaVerificada` | `Boolean` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaHabilitado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[SalidaDeCentro]
    A1[GuardarFechaEgreso]
    A2[Visteo]
    A3[AsignacionTarjetaDeAcceso]
    A4[EnPlayaExterna]
    A5[ImprimeReciboMunicipal]
    A6[EnTransito]
    A7[AutorizarTiempoEnTransito]
    A8[PuestoComando]
    A9[PesadaTara]
    A10[BalanzaACero]
    A11[RefrescarOrdenCargaInternaFason]
    A12[PesadaBruto]
    A13[VerificacionCamionRechazado]
    A14[BalanzaACero]
    A15[ImpresionFormulario239]
    A16[ControlPesoMaximo]
    A17[BalanzaACero]
    A18[EnEsperaIndianapolis]
    A19[CargarPrecintos]
    A20[IngresoDeObservaciones]
    A21[ImpresionTicketPesada]
    A22[GuardarFechaEgreso]
    A23[ServicioSapMov291]
    A24[VerificacionCot]
    A25[SalidaDeCentro]
    A26[AutorizarTransportistaInhabilitado]
    A27[VerificacionTransportistaHabilitado]
    A28[IngresoDeTransportistaOrdenCargaInternaFason]
    A29[VerificacionTransportistaExiste]
    A30[IngresarOrdenCargaInternaFason]
    D31{"transportistaAutorizado"}
    D32{"vehiculoEnCondiciones"}
    D33{"autorizarTiempo"}
    D34{"camionRechazado"}
    D35{"excedePesoMaximo"}
    D36{"debeRepesar"}
    D37{"transportistaHabilitado"}
    D38{"existeTransportista"}
    A25 --> A22
    A22 --> A2
    A2 --> A3
    A3 --> A4
    A4 --> A5
    A5 --> A6
    A6 --> A7
    A7 --> A8
    A8 --> A9
    A9 --> A17
    A17 --> A11
    A11 --> A12
    A12 --> A13
    A13 --> A17
    A17 --> A15
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
| 1 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 2 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 3 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 4 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 5 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 6 | `ImprimeReciboMunicipal` | workflowId, materialId, puestoDeTrabajoId, centroId, codigoDeImpresion |
| 7 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 8 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 9 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 10 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 11 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 12 | `RefrescarOrdenCargaInternaFason` | workflowId, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 13 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 14 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 15 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 16 | `ImpresionFormulario239` | NumeroDeFormulario, CentroId, CodigoDeImpresion, Observaciones, Patente |
| 17 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 18 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 19 | `EnEsperaIndianapolis` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 20 | `CargarPrecintos` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 21 | `IngresoDeObservaciones` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 22 | `ImpresionTicketPesada` | Observaciones, PuestoDeTrabajoId, MaterialId, NumeroDocumento, WorkflowId |
| 23 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 24 | `ServicioSapMov291` | nombreUsuario, instanceId, cantidad, puestoDeTrabajoId, materialId |
| 25 | `VerificacionCot` | hndInstance, puestoDeTrabajoId, instanceId, nombreUsuario |
| 26 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 27 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |
| 28 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 29 | `IngresoDeTransportistaOrdenCargaInternaFason` | workflowId, ordenCargaInternaFason, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 30 | `VerificacionTransportistaExiste` | WorkflowId, Result, PuestoDeTrabajoId, TransportistaId |
| 31 | `IngresarOrdenCargaInternaFason` | hndInstance, tipoDocumentoIngreso, centroId, orden, numeroDocumentoIngreso |

**Decisiones de flujo:**

- **Autoriza?**: `[transportistaAutorizado]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **Vehículo Rechazado?**: `[camionRechazado]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Repesa vehículo?**: `[debeRepesar]`
- **Transportista habilitado?**: `[transportistaHabilitado]`
- **Existe el Transportista?**: `[existeTransportista]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `CargarPrecintos`
- `ControlPesoMaximo`
- `EnEsperaIndianapolis`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionFormulario239`
- `ImpresionFormulario239.EmpresaCuit`
- `ImpresionFormulario239.EmpresaDescripcion`
- `ImpresionFormulario239.Representante`
- `ImpresionTicketPesada`
- `ImpresionTicketPesada.CodigoDeImpresion`
- `ImprimeReciboMunicipal`
- `IngresarOrdenCargaInternaFason`
- `IngresoDeObservaciones`
- `IngresoDeTransportistaOrdenCargaInternaFason`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarOrdenCargaInternaFason`
- `SalidaDeCentro`
- `ServicioSapMov291`
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