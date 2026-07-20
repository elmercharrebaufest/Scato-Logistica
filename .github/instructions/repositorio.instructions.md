---
applyTo: "**/Molinos.Scato.Repositorio/RepositorioEF.cs"
---

# Reglas de RepositorioEF (Repositorio)

## Principio fundamental
`RepositorioEF` es la **única implementación de `IRepositorio`**. Es la puerta de entrada a la base de datos vía Entity Framework 5. Solo puede depender de `Molinos.Scato.Dominio` y Entity Framework. No contiene lógica de negocio.

## Operaciones disponibles en `IRepositorio`

### Consultas de una entidad
```csharp
repositorio.Obtener<Almacen>(id)                          // por PK
repositorio.Obtener<Almacen>(e => e.Descripcion == desc)  // por predicado (único)
repositorio.ObtenerPrimero<Almacen>(e => e.CentroId == id) // primero que coincide
repositorio.ObtenerMasReciente<Almacen>(filtro, e => e.Fecha) // más reciente por fecha
repositorio.ObtenerProyeccion<Almacen, AlmacenDto>(filtro, proyeccion)
repositorio.ObtenerUnchanged<Almacen>(id) // trae la entidad con estado Unchanged (para update sin tracking)
```

### Consultas de lista
```csharp
repositorio.Listar<Almacen>()                             // todos
repositorio.Listar<Almacen>(e => e.Centro.Id == centroId) // filtrados
repositorio.Listar<Almacen>(includes, filtro)             // con eager loading
repositorio.ListarNoTracking<Almacen>(filtro)             // sin tracking EF (read-only)
repositorio.Listar<Almacen, AlmacenDto>(proyeccion, filtro) // proyectando a DTO
repositorio.Listar<Almacen>(filtro, maxResultados)        // con límite
```

### Consultas paginadas
```csharp
repositorio.Listar<Almacen>(filtro, paginacion)
repositorio.Listar<Almacen, AlmacenDto>(proyeccion, filtro, paginacion)
```

### Consultas complejas (delegar a `ConsultasEF/`)
```csharp
repositorio.ListarConsultaPaginada(new ListarXxxConsulta(filtro, paginacion))
repositorio.ListarConsulta(new ListarXxxConsulta(filtro))
repositorio.ObtenerConsultaEscalar(new ObtenerXxxConsulta(parametros))
```

### Verificaciones
```csharp
repositorio.Existe<Almacen>(e => e.Descripcion == desc)
repositorio.Contar<Almacen>()
repositorio.Contar<Almacen>(e => e.Centro.Id == centroId)
repositorio.Sumar<Almacen>(e => e.Capacidad, filtro)
```

### Mutaciones
```csharp
repositorio.Agregar(entidad)
repositorio.Remover(entidad)
repositorio.Remover<Almacen>(id)
repositorio.RemoverTodos(lista)
repositorio.GuardarCambios()   // flush a la BD — llamar siempre al final de la unidad de trabajo
```

## `ScatoDbContext`

- Es el único `DbContext` del proyecto. Connection string key: `ScatoDb`.
- Mapea automáticamente **todos los tipos** del namespace `Molinos.Scato.Dominio.Entidades` — no es necesario agregar `DbSet<T>` para nuevas entidades.
- La pluralización de tabla está **deshabilitada** — el nombre de la tabla = nombre de la clase.
- Relaciones especiales (many-to-many sin tabla explícita, precisión decimal, etc.) se configuran en `OnModelCreating()`.
- No agregar lógica de negocio a `ScatoDbContext`.

## Scope en DI

`DbContext` e `IRepositorio` se registran con `InScope(ctx => OperationContext.Current)` en `ServiciosWebNinjectModule` — una instancia por llamada WCF. No usar Singleton ni Transient.

## `EntidadReferenciadaException`

Se lanza cuando se intenta eliminar una entidad que tiene FK activas (error SQL 547). Reutilizar esta excepción en lugar de crear nuevas para el mismo escenario.

## Cuándo usar consultas complejas en lugar de `IRepositorio`

Usar `ConsultasEF/` cuando:
- La query requiere SQL raw por performance o funcionalidad no expresable en LINQ.
- La consulta involucra múltiples JOINs complejos, subqueries correlacionadas, o CTEs.
- Se necesita cambiar el timeout de la operación (`CommandTimeout`).
- La lógica de la query tiene más de ~5 cláusulas `Where`/`Select` encadenadas.

## Prohibido en esta capa
- Lógica de negocio o validaciones de dominio.
- SQL en strings dentro de `RepositorioEF`.
- Referencias a `Molinos.Scato.Servicios`, `Molinos.Scato.Web`, o cualquier host.
- Múltiples `DbContext` en el mismo proyecto.
- Implementaciones alternativas de `IRepositorio` fuera de testing.
