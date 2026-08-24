# Workflow: SLO_RecepcionClienteFason

> **Archivo:** `SLO.RecepcionClienteFason.xamlx`  
> **Tipo de raiz:** Flowchart  
> **Generado:** 2026-08-06 18:43  
> **Actividades custom:** 30  
> **Variables de scope:** 23  

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
| `impresora` | `String` | [System.Configuration.ConfigurationManager.AppSettings("Impresora")] |
| `instanceId` | `Guid` | -- |
| `nombreUsuario` | `String` | nombreTest |
| `observacion` | `String` | -- |
| `orden` | `OrdenDeDescargaFasonDto` | -- |
| `pesoBruto` | `Int32` | -- |
| `pesoTara` | `Int32` | -- |
| `rechazar` | `Boolean` | -- |
| `tipoDocumentoIngreso` | `TipoDocumentoIngreso` | -- |
| `transportistaAutorizado` | `Boolean` | -- |
| `transportistaHabilitado` | `Boolean` | -- |
| `ultimoPuestoDeTrabajoId` | `Int32` | -- |

## Diagrama del flujo

```mermaid
flowchart TD
    A0[CargarOrdenDeDescargaFason]
    A1[ControlDeIngreso]
    A2[GuardarFechaEgreso]
    A3[SalidaDeCentro]
    A4[VerificacionTransportistaExiste]
    A5[VerificacionTransportistaHabilitado]
    A6[AsignacionTarjetaDeAcceso]
    A7[EnPlayaExterna]
    A8[ImprimeReciboMunicipal]
    A9[EnTransito]
    A10[AutorizarTiempoEnTransito]
    A11[PuestoComando]
    A12[PesadaBruto]
    A13[RefrescarOrdenDeDescargaFason]
    A14[ImpresionAsignacionDeRuta]
    A15[ControlPesoMaximo]
    A16[BalanzaACero]
    A17[ConfirmacionDeCargaDescarga]
    A18[PesadaTara]
    A19[VerificacionCamionRechazado]
    A20[BalanzaACero]
    A21[GuardarFechaEgreso]
    A22[ImpresionFormulario239]
    A23[SalidaDeCentro]
    A24[IngresoDeObservaciones]
    A25[ImpresionCertificadoDeCartaPorte]
    A26[ServicioSapIngresosEgresosFazones]
    A27[AutorizarTransportistaInhabilitado]
    A28[IngresoDeTransportistaOrdenDeDescargaFason]
    D29{"rechazar"}
    D30{"existeTransportista"}
    D31{"transportistaHabilitado"}
    D32{"autorizarTiempo"}
    D33{"excedePesoMaximo"}
    D34{"debeRepesar"}
    D35{"camionRechazado"}
    D36{"transportistaAutorizado"}
    A0 --> A1
    A1 --> A21
    A21 --> A23
    A23 --> A4
    A4 --> A5
    A5 --> A6
    A6 --> A7
    A7 --> A8
    A8 --> A9
    A9 --> A10
    A10 --> A11
    A11 --> A12
    A12 --> A13
    A13 --> A14
    A14 --> A15
    A15 --> A20
    A20 --> A17
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
```

## Secuencia de actividades

| # | Actividad | Argumentos clave |
|---|-----------|-----------------|
| 1 | `CargarOrdenDeDescargaFason` | hndInstance, tipoDocumentoIngreso, centroId, orden, fechaInicio |
| 2 | `ControlDeIngreso` | hndInstance, ordenDeDescargaFason, rechazar, workflowId, puestoDeTrabajoId |
| 3 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 4 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 5 | `VerificacionTransportistaExiste` | WorkflowId, Result, PuestoDeTrabajoId, TransportistaId |
| 6 | `VerificacionTransportistaHabilitado` | choferId, centroId, patente, workflowId, transportistaValido |
| 7 | `AsignacionTarjetaDeAcceso` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 8 | `EnPlayaExterna` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 9 | `ImprimeReciboMunicipal` | workflowId, materialId, puestoDeTrabajoId, centroId, codigoDeImpresion |
| 10 | `EnTransito` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 11 | `AutorizarTiempoEnTransito` | autorizado, centroId, codigoControl, patente, workflowId |
| 12 | `PuestoComando` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 13 | `PesadaBruto` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 14 | `RefrescarOrdenDeDescargaFason` | workflowId, tipoDocumentoIngreso, puestoDeTrabajoId, orden |
| 15 | `ImpresionAsignacionDeRuta` | Calado, CentroId, CodigoDeImpresion, Patente, MaterialId |
| 16 | `ControlPesoMaximo` | hndInstance, excedePesoMaximo, repesar, workflowId, soloNotifica |
| 17 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 18 | `ConfirmacionDeCargaDescarga` | workflowId, hndInstance, puestoDeTrabajoId, NombreUsuario |
| 19 | `PesadaTara` | hndInstance, fecha, workflowId, peso, puestoDeTrabajoId |
| 20 | `VerificacionCamionRechazado` | hndInstance, Observacion, camionRechazado, workflowId, pesoNeto |
| 21 | `BalanzaACero` | puestoDeTrabajoId, hndInstance, balanzaId, instanceId, nombreUsuario |
| 22 | `GuardarFechaEgreso` | WorkflowId, FechaEgreso, PuestoDeTrabajoId |
| 23 | `ImpresionFormulario239` | NumeroDeFormulario, CentroId, CodigoDeImpresion, Observaciones, Patente |
| 24 | `SalidaDeCentro` | hndInstance, centroId, patente, workflowId, puestoDeTrabajoId |
| 25 | `IngresoDeObservaciones` | workflowId, hndInstance, puestoDeTrabajoId, nombreUsuario |
| 26 | `ImpresionCertificadoDeCartaPorte` | PesoNetoOrigen, PuestoDeTrabajoId, FechaEntrada, NumeroDocumento, WorkflowId |
| 27 | `ServicioSapIngresosEgresosFazones` | nombreUsuario, TipoMovimiento, instanceId, vehiculoId, FechaIngreso |
| 28 | `AutorizarTransportistaInhabilitado` | autorizado, choferId, centroId, patente, workflowId |
| 29 | `IngresoDeTransportistaOrdenDeDescargaFason` | workflowId, OrdenDeDescargaFason, hndInstance, puestoDeTrabajoId, nombreUsuario |

**Decisiones de flujo:**

- **Rechaza Vehículo?**: `[rechazar]`
- **Existe el Transportista?**: `[existeTransportista]`
- **Transportista habilitado?**: `[transportistaHabilitado]`
- **¿Autoriza Tiempo?**: `[autorizarTiempo]`
- **Excede Peso Máximo?**: `[excedePesoMaximo]`
- **Se Repesa Camión?**: `[debeRepesar]`
- **Vehículo Rechazado?**: `[camionRechazado]`
- **Autoriza?**: `[transportistaAutorizado]`

## Actividades custom referenciadas

> Ensamblado: ``Molinos.Scato.Actividades``

- `AsignacionTarjetaDeAcceso`
- `AutorizarTiempoEnTransito`
- `AutorizarTransportistaInhabilitado`
- `BalanzaACero`
- `CargarOrdenDeDescargaFason`
- `ConfirmacionDeCargaDescarga`
- `ControlDeIngreso`
- `ControlPesoMaximo`
- `EnPlayaExterna`
- `EnTransito`
- `GuardarFechaEgreso`
- `ImpresionAsignacionDeRuta`
- `ImpresionAsignacionDeRuta.Calidad`
- `ImpresionCertificadoDeCartaPorte`
- `ImpresionFormulario239`
- `ImpresionFormulario239.EmpresaCuit`
- `ImpresionFormulario239.EmpresaDescripcion`
- `ImpresionFormulario239.Representante`
- `ImprimeReciboMunicipal`
- `IngresoDeObservaciones`
- `IngresoDeTransportistaOrdenDeDescargaFason`
- `PesadaBruto`
- `PesadaTara`
- `PuestoComando`
- `RefrescarOrdenDeDescargaFason`
- `SalidaDeCentro`
- `ServicioSapIngresosEgresosFazones`
- `VerificacionCamionRechazado`
- `VerificacionTransportistaExiste`
- `VerificacionTransportistaHabilitado`

## Dependencias de dominio

- `Molinos.Scato.Actividades`
- `Molinos.Scato.Dependencias`
- `Molinos.Scato.Dominio`
- `Molinos.Scato.Servicios`
- `Molinos.Scato.Workflow`

---
_Documentacion generada automaticamente por `AiEnablement/Generate-WfDocs.ps1`._