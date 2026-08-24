# Workflow: SLO_EgresoPorExportaciones

> **Archivo:** `SLO.EgresoPorExportaciones.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 24  
> **Variables de scope:** 24  

## Descripcion de negocio

_TODO: Agregar descripcion del proceso de negocio._

## Variables de scope

| Nombre | Tipo | Valor por defecto |
|--------|------|-------------------|
| `autorizarTiempo` | `Boolean` | -- |
| `balanzaId` | `Int32` | -- |
| `calado` | `CaladoDto` | -- |
| `caladoPorCaracteristica` | `CaladoPorCaracteristicaDto[]` | -- |
| `camionRechazado` | `Boolean` | -- |
| `centroId` | `Int32` | -- |
| `envioACamaraObligatorio` | `Boolean` | -- |
| `excedePesoMaximo` | `Boolean` | -- |
| `fechaBruto` | `DateTime` | -- |
| `fechaTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `nombreUsuario` | `String` | -- |
| `numeroDocumentoIngreso` | `String` | -- |
| `observacion` | `String` | -- |
| `orden` | `OrdenCargaInternaDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `repesar` | `Boolean` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |
| `vehiculoRechazado` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[IngresarOrdenCargaInterna]
    A1[VerificacionTransportistaHabilitado]
    A2[Visteo]
    A3[AsignacionTarjetaDeAcceso]
    A4[EnPlayaExterna]
    A5[ImprimeReciboMunicipal]
    A6[EnTransito]
    A7[AutorizarTiempoEnTransito]
    A8[PuestoComando]
    A9[PesadaTara]
    A10[BalanzaACero]
    A11[PesadaBruto]
    A12[VerificacionCamionRechazado]
    A13[BalanzaACero]
    A14[ImpresionFormulario239]
    A15[GuardarFechaEgreso]
    A16[SalidaDeCentro]
    A17[ControlPesoMaximo]
    A18[BalanzaACero]
    A19[EnEsperaIndianapolis]
    A20[ControlPesoMaximo]
    A21[ServicioSapMov291]
    A22[VerificacionCot]
    A23[EnEsperaAduana]
    A24[AutorizarTransportistaInhabilitado]
    D25{"transportistaAutorizado"}
    D26{"vehiculoEnCondiciones"}
    D27{"autorizarTiempo"}
    D28{"camionRechazado"}
    D29{"excedePesoMaximo"}
    D30{"repesar"}
    D31{"transportistaAutorizado"}
    A0 --> A1
    A1 --> A2
    A2 --> A3
    A3 --> A4
    A4 --> A5
    A5 --> A6
    A6 --> A7
    A7 --> A8
    A8 --> A9
    A9 --> A18
    A18 --> A11
    A11 --> A12
    A12 --> A18
    A18 --> A14
    A14 --> A15
    A15 --> A16
    A16 --> A20
    A20 --> A18
    A18 --> A19
    A19 --> A20
    A20 --> A21
    A21 --> A22
    A22 --> A23
    A23 --> A24
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `IngresarOrdenCargaInterna` | hndInstance, tipoDocumentoIngreso, centroId, orden, numeroDocumentoIngreso |
| 2 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 3 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 4 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 5 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 6 | `ImprimeReciboMunicipal` | workflowId, materialId, puestoDeTrabajoId, centroId, codigoDeImpresion |
| 7 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 8 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 9 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 10 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 11 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 12 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 13 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 14 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 15 | `ImpresionFormulario239` | Observaciones, PuestoDeTrabajoId, MaterialId, WorkflowId, CodigoDeImpresion |
| 16 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 17 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 18 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 19 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 20 | `EnEsperaIndianapolis` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 21 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 22 | `ServicioSapMov291` | nombreUsuario, instanceId, cantidad, puestoDeTrabajoId, materialId |
| 23 | `VerificacionCot` | hndInstance, puestoDeTrabajoId, instanceId, nombreUsuario |
| 24 | `EnEsperaAduana` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 25 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |

**Decisiones de flujo:**

- **¿Transportista Habilitado?**: `[transportistaAutorizado]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **¿Camión Rechazado?**: `[camionRechazado]`
- **¿Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Repesa vehículo?**: `[repesar]`
- **¿Transportista Autorizado?**: `[transportistaAutorizado]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `ControlPesoMaximo`
- `EnEsperaAduana`
- `EnEsperaIndianapolis`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionFormulario239`
- `ImpresionFormulario239.EmpresaCuit`
- `ImpresionFormulario239.Representante`
- `ImprimeReciboMunicipal`
- `IngresarOrdenCargaInterna`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `SalidaDeCentro`
- `ServicioSapMov291`
- `VerificacionCamionRechazado`
- `VerificacionCot`
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