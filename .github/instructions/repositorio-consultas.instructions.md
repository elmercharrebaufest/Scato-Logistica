---
applyTo: "**/Molinos.Scato.Repositorio/ConsultasEF/*.cs"
---

# Reglas de ConsultasEF (Repositorio)

## Principio fundamental
`ConsultasEF/` contiene clases que encapsulan **operaciones complejas de acceso a datos** que no son expresables limpiamente con la API genérica de `IRepositorio`. Reciben directamente el `DbContext` y pueden usar LINQ avanzado o SQL raw.

## Contratos disponibles

| Interfaz | Método | Carpeta | Cuándo usar |
|----------|--------|---------|-------------|
| `IConsultaPaginada<TResult>` | `ListaPaginada<TResult> Ejecutar(DbContext)` | `ConsultasEF/` | Query paginada con proyección a DTO |
| `IConsulta<TResult>` | `List<TResult> Ejecutar(DbContext)` | `ConsultasEF/` | Query de lista sin paginación |
| `IConsultaEscalar<TResult>` | `TResult Ejecutar(DbContext)` | `ConsultasEF/` | Query que retorna un único valor u objeto |

## Estructura de una consulta paginada

```csharp
namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarNuevosConceptosConsulta : IConsultaPaginada<NuevoConceptoDto>
    {
        private readonly FiltroNuevoConceptoDto filtro;
        private readonly Paginacion paginacion;

        public ListarNuevosConceptosConsulta(FiltroNuevoConceptoDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<NuevoConceptoDto> Ejecutar(DbContext contexto)
        {
            var query = contexto.Set<NuevoConcepto>()
                .Where(x => x.Centro.Id == filtro.CentroId);

            if (!string.IsNullOrWhiteSpace(filtro.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(filtro.Descripcion));

            var total = query.Count();
            var items = query
                .OrderBy(x => x.Descripcion)
                .Skip(paginacion.Skip)
                .Take(paginacion.PageSize)
                .Select(x => new NuevoConceptoDto { Id = x.Id, Descripcion = x.Descripcion })
                .ToList();

            return new ListaPaginada<NuevoConceptoDto>(items, total, paginacion);
        }
    }
}
```

## Cómo ejecutar desde un procesador o servicio

```csharp
// Consulta paginada
var resultado = repositorio.ListarConsultaPaginada(
    new ListarNuevosConceptosConsulta(filtro, paginacion));

// Consulta de lista
var items = repositorio.ListarConsulta(
    new ListarXxxConsulta(parametros));

// Consulta escalar
var dto = repositorio.ObtenerConsultaEscalar(
    new ObtenerVehiculo(patente, acoplado, acoplado2));

// Comando con retorno
var id = repositorio.EjecutarComando(
    new CrearLoteAuditoria(materialId, centroId));
```

## Convenciones de naming

| Tipo | Sufijo de archivo | Ejemplo |
|------|------------------|---------|
| `IConsultaPaginada<T>` | `{Concepto}Consulta.cs` | `ListarWorkFlowsConsulta.cs` |
| `IConsulta<T>` | `{Concepto}Consulta.cs` | `PermisosPorUsuarioConsulta.cs` |
| `IConsultaEscalar<T>` | nombre descriptivo | `ObtenerVehiculo.cs`, `ObtenerCalle.cs` |

## Cuándo usar SQL raw vs LINQ

- **Preferir LINQ** para queries que EF puede traducir eficientemente.
- **Usar `Database.SqlQuery<T>()`** cuando: se necesita SQL específico (CTEs, hints, funciones de ventana), la performance de EF es inadecuada, o la query involucra procedimientos almacenados.
- Usar **`SqlParameter`** siempre para los parámetros — nunca concatenar valores en el SQL.

## Timeout de operación

Para consultas lentas o de larga duración:
```csharp
((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180; // segundos
```

## Prohibido en esta capa
- Lógica de negocio — solo acceso a datos.
- Llamadas a servicios externos.
- Concatenar parámetros de usuario directamente en strings SQL (riesgo de SQL injection).
- Usar `TransactionScope` salvo que sea estrictamente necesario para la consistencia de la operación.
