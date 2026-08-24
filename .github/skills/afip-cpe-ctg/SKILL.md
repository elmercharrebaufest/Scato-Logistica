---
name: afip-cpe-ctg
description: Referencia de operaciones AFIP para CTG (Constancia de Transporte de Granos) y CPE (Carta de Porte Electronica) automotor. Usar al implementar o debuggear integracion con AFIP en este proyecto.
---

# AFIP CTG/CPE — Referencia de operaciones

## Regla de arquitectura

**Toda llamada AFIP va en `Procesamiento/`** — nunca desde controllers, actividades ni workflows directamente.  
Clase base: `ProcesadorComando<TComando>` → `Ejecutar(comando)` → retorna `Resultado`.

## CTG — AfipCTGWebService

Proxy inyectado: `CTGServicePortType` + `IAccesoWsCtg.ObtenerAuthType(cuitSinGuiones, resultado)`.

| Operación WS | Método proxy | Uso |
|---|---|---|
| `solicitarCTGInicial` | `serviceAfipCTG.solicitarCTGInicial(request)` | Crear CTG → devuelve `ctg` (long) |
| `confirmarArribo` | `serviceAfipCTG.confirmarArribo(request)` | Confirmar llegada al destino |
| `darDeBajaCTG` | `serviceAfipCTG.darDeBajaCTG(request)` | Cerrar/anular CTG |
| `consultarCTGActivosPorPatente` | `serviceAfipCTG.consultarCTGActivosPorPatente(request)` | Fallback cuando alta devuelve ctg = 0 |

**Flujo CTG:** `solicitarCTGInicial` → `confirmarArribo` → `darDeBajaCTG`

## CPE — AfipCPDigitalService

Proxy inyectado: `CPDigitalServicePortType` + `IAccesoWsCpe.ObtenerAuthType(...)`.

| Operación WS | Uso |
|---|---|
| `autorizarCPEAutomotor` | Crear CPE automotor |
| `confirmarArriboCPE` | Confirmar arribo |
| `cerrarCPE` | Cierre definitivo |
| `anularCPE` | Anulación |
| `consultarCPEAutomotor` | Consulta estado actual |

**Flujo CPE:** `autorizarCPEAutomotor` → `confirmarArriboCPE` → `cerrarCPE` | `anularCPE`

## Formato de campos clave

| Campo | Tipo C# | Transformación |
|---|---|---|
| CUIT/CUIL | `long` | `.Replace("-", "")` → `Convert.ToInt64` |
| Código cosecha | `string` | `.Replace("-", "")` — ej: `"23-24"` → `"2324"` |
| Código localidad AFIP | `int` | `Convert.ToInt32(localidad.CodigoAfip)` |
| Número carta porte | `long` | `(long)Convert.ToDouble(nroCartaPorte)` |
| Peso neto | `int` | kg enteros; usar `0` si `null` |

## Manejo de errores — patrón estándar

```csharp
// 1. Siempre verificar arrayErrores primero
if (response.response.arrayErrores.Any())
{
    resultado.Errores.Add("Clave", response.response.arrayErrores.FirstOrDefault());
    return resultado;
}
// 2. Verificar arrayControles si datos es null (respuesta parcial AFIP)
if (response.response.datosSolicitarCTGResponse.arrayControles != null
    && response.response.datosSolicitarCTGResponse.datosSolicitarCTG == null)
{
    resultado.Errores.Add("Clave", response.response.datosSolicitarCTGResponse.arrayControles.FirstOrDefault()?.descripcion);
    return resultado;
}
// 3. Validar que ctg > 0 — si es 0, usar consultarCTGActivosPorPatente como fallback
```

## Entidades de persistencia

| Entidad | Cuándo crear | Campos obligatorios |
|---|---|---|
| `AltaCTG` | Alta exitosa | `CartaPorte`, `CodigoCTG`, `Fecha`, `WorkflowId` |
| `BajaCTG` | Baja exitosa | según DTO de respuesta |
| `CartaPorte.CTG` | Actualizar tras alta | `cartaPorte.CTG = ctg` |
| `CartaPorte.TarifaReferencia` | Actualizar tras alta | `datos.datosSolicitarCTG.tarifaReferencia` |

## Configuración y SSL

```csharp
// SSL legacy — no remover, AFIP no valida certificado en todos los ambientes
System.Net.ServicePointManager.ServerCertificateValidationCallback = ((s, c, ch, e) => true);

// Log de requests (configurable)
// AppSettings["LoguearRequestsCtg"] = "1" → guarda XML en ControlRecorrido
```
