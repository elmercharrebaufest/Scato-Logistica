---
applyTo: "**/Molinos.Scato.Repositorio/RepositorioEF.cs"
---

# Capa RepositorioEF (Repositorio)

## Objetivo
Implementar `IRepositorio` como puerta única de acceso a datos con EF5.

## Hacer
- Centralizar consultas y mutaciones en `RepositorioEF`.
- Usar `ListarNoTracking`/proyecciones para lectura.
- Delegar consultas complejas a `ConsultasEF`.
- Mantener `ScatoDbContext` como único `DbContext`.
- Reusar `EntidadReferenciadaException` para FK activas en delete.

## No hacer
- No agregar lógica de negocio.
- No exponer dependencias hacia capas web/servicios host.
- No crear implementaciones alternativas de repositorio en producción.
- No usar SQL concatenado con parámetros de usuario.

## Ejemplo mínimo
```csharp
var existe = repositorio.Existe<Almacen>(x => x.Descripcion == descripcion);
if (!existe)
{
    repositorio.Agregar(new Almacen { Descripcion = descripcion });
    repositorio.GuardarCambios();
}
```
