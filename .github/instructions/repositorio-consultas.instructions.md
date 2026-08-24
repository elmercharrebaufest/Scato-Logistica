---
applyTo: "**/Molinos.Scato.Repositorio/ConsultasEF/*.cs"
---

# Capa ConsultasEF (Repositorio)

## Objetivo
Encapsular consultas complejas de datos que no conviene resolver con API genérica del repositorio.

## Hacer
- Implementar el contrato correcto:
  - `IConsultaPaginada<T>`
  - `IConsulta<T>`
  - `IConsultaEscalar<T>`
- Mantener foco en acceso a datos (LINQ/SQL parametrizado).
- Usar proyección y paginación cuando aplique.
- Ajustar `CommandTimeout` solo en consultas realmente lentas.

## No hacer
- No incluir reglas de negocio.
- No llamar servicios externos.
- No concatenar parámetros en SQL.
- No usar transacciones amplias sin necesidad.

## Ejemplo mínimo
```csharp
public class ListarAlmacenesConsulta : IConsulta<AlmacenDto>
{
    public List<AlmacenDto> Ejecutar(DbContext contexto)
    {
        return contexto.Set<Almacen>()
            .AsNoTracking()
            .Select(x => new AlmacenDto { Id = x.Id, Descripcion = x.Descripcion })
            .ToList();
    }
}
```
