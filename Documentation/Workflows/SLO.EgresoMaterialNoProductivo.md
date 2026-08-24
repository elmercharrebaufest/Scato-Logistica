# Workflow: SLO_EgresoMaterialNoProductivo

> **Archivo:** `SLO.EgresoMaterialNoProductivo.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 26  
> **Variables de scope:** 22  

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
| `fechaBruto` | `DateTime` | -- |
| `fechaInicio` | `DateTime` | -- |
| `fechaTara` | `DateTime` | -- |
| `hndInstance` | `CorrelationHandle` | -- |
| `instanceId` | `Guid` | -- |
| `nombreUsuario` | `String` | NombreTest |
| `numeroDocumentoIngreso` | `String` | -- |
| `Observacion` | `String` | -- |
| `orden` | `OrdenCargaInternaDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaHabilitado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |
| `vehiculoEnCondiciones` | `Boolean` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[Visteo]
    A1[AsignacionTarjetaDeAcceso]
    A2[EnPlayaExterna]
    A3[ImprimeReciboMunicipal]
    A4[EnTransito]
    A5[AutorizarTiempoEnTransito]
    A6[PuestoComando]
    A7[PesadaTara]
    A8[BalanzaACero]
    A9[RefrescarOrdenCargaInterna]
    A10[PesadaBruto]
    A11[VerificacionCamionRechazado]
    A12[BalanzaACero]
    A13[ImpresionFormulario239]
    A14[GuardarFechaEgreso]
    A15[SalidaDeCentro]
    A16[ControlPesoMaximo]
    A17[BalanzaACero]
    A18[EnEsperaIndianapolis]
    A19[ImpresionCertificadoDeCartaPorte]
    A20[ServicioSapEgresosNoProductivos]
    A21[VerificacionCot]
    A22[GuardarFechaEgreso]
    A23[SalidaDeCentroAutomatica]
    A24[AutorizarTransportistaInhabilitado]
    A25[VerificacionTransportistaHabilitado]
    A26[IngresarOrdenCargaInterna]
    D27{"transportistaHabilitado"}
    D28{"vehiculoEnCondiciones"}
    D29{"autorizarTiempo"}
    D30{"camionRechazado"}
    D31{"excedePesoMaximo"}
    D32{"debeRepesar"}
    D33{"transportistaAutorizado"}
    A0 --> A1
    A1 --> A2
    A2 --> A3
    A3 --> A4
    A4 --> A5
    A5 --> A6
    A6 --> A7
    A7 --> A17
    A17 --> A9
    A9 --> A10
    A10 --> A11
    A11 --> A17
    A17 --> A13
    A13 --> A22
    A22 --> A15
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
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `Visteo` | vehiculoEnCondiciones, observacion, workflowId, hndInstance, puestoDeTrabajoId |
| 2 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 3 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 4 | `ImprimeReciboMunicipal` | centroId, codigoDeImpresion, materialId, workflowId, puestoDeTrabajoId |
| 5 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 6 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 7 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 8 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 9 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 10 | `RefrescarOrdenCargaInterna` | workflowId, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 11 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 12 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 13 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 14 | `ImpresionFormulario239` | Observaciones, PuestoDeTrabajoId, MaterialId, WorkflowId, CodigoDeImpresion |
| 15 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 16 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 17 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 18 | `BalanzaACero` | hndInstance, tipoVehiculo, balanzaId, puestoDeTrabajoId, instanceId |
| 19 | `EnEsperaIndianapolis` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 20 | `ImpresionCertificadoDeCartaPorte` | PesoNetoOrigen, PuestoDeTrabajoId, FechaEntrada, NumeroDocumento, WorkflowId |
| 21 | `ServicioSapEgresosNoProductivos` | nombreUsuario, instanceId, puestoDeTrabajoId, MaterialId, hndInstance |
| 22 | `VerificacionCot` | hndInstance, puestoDeTrabajoId, instanceId, nombreUsuario |
| 23 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 24 | `SalidaDeCentroAutomatica` | workflowId, puestoDeTrabajoId, nombreUsuario |
| 25 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |
| 26 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 27 | `IngresarOrdenCargaInterna` | hndInstance, tipoDocumentoIngreso, centroId, orden, numeroDocumentoIngreso |

**Decisiones de flujo:**

- **Transportista habilitado?**: `[transportistaHabilitado]`
- **Vehículo En Condiciones?**: `[vehiculoEnCondiciones]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **Camión Rechazado?**: `[camionRechazado]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Se Repesa Camión?**: `[debeRepesar]`
- **Autoriza?**: `[transportistaAutorizado]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `ControlPesoMaximo`
- `EnEsperaIndianapolis`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionCertificadoDeCartaPorte`
- `ImpresionFormulario239`
- `ImpresionFormulario239.EmpresaCuit`
- `ImpresionFormulario239.Representante`
- `ImprimeReciboMunicipal`
- `IngresarOrdenCargaInterna`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarOrdenCargaInterna`
- `SalidaDeCentro`
- `SalidaDeCentroAutomatica`
- `ServicioSapEgresosNoProductivos`
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