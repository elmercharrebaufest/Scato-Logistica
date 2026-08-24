---
name: entity-scaffold
description: "Scaffold para agregar un caso de uso nuevo a una entidad: comando, procesador, consulta EF5 y tests NUnit."
---

# Entity scaffold

## Cuando usarlo

Usar cuando hace falta crear un flujo nuevo sobre una entidad existente o nueva y conectar Dominio, Repositorio, Servicios y Tests.

## Salidas esperadas

| Artefacto | Ruta tipica | Nota |
|---|---|---|
| Comando | `Molinos.Scato.Dominio/Comandos/{Accion}{Entidad}.cs` | Describe la intencion |
| Procesador | `Molinos.Scato.Servicios/Procesamiento/Procesador{Accion}{Entidad}.cs` | Ejecuta negocio y persistencia |
| Consulta EF5 | `Molinos.Scato.Repositorio/ConsultasEF/{Nombre}Consulta.cs` | Solo si el caso es de lectura/paginacion |
| Test | `Molinos.Scato.Test/Procesamiento/Procesador{Accion}{Entidad}Test.cs` | NUnit 2.6.3 + Moq |

## Reglas del repo

- Respetar capas: Web -> Servicios -> Repositorio -> Dominio.
- El procesador suele inyectar `IRepositorio`, `IConversor` y `ILogger`.
- Los tests del procesador usan `NullLogger` y mockean `IRepositorio` / `IConversor`.
- Para consultas read-only usar `DbContext`, `AsNoTracking()` y proyeccion a DTO.
- En EF5 no usar `ToListAsync` ni `SaveChangesAsync`.
- Si el CRUD ya existe, crear un caso de uso nuevo; no duplicar el flujo exacto.

## Checklist de generacion

1. Leer la entidad, el DTO y el filtro existentes.
2. Revisar si ya hay procesadores similares para copiar patrones.
3. Elegir la base correcta: `ProcesadorCrear`, `ProcesadorModificar`, `ProcesadorEliminar` o `ProcesadorComando`.
4. Si hay lectura paginada, crear una consulta en `ConsultasEF` con `IConsultaPaginada<TDto>`.
5. Crear tests que verifiquen alta, validacion y persistencia.

## Reglas de salida

- Validaciones de negocio en el procesador con `Resultado.Error("Campo", Textos.Clave)`.
- Consulta EF solo para lectura; nunca mutar entidades ahi.
- Tests con nombre del tipo `Procesador{Accion}{Entidad}Test`.
- Mantener nombres y rutas alineados con los existentes en el repo.
