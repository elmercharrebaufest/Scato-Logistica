---
applyTo: "**/Molinos.Scato.Web/Controllers/*.cs"
---

# Capa Web Controllers

## Objetivo
Orquestar request/response HTTP delegando negocio a servicios.

## Hacer
- Heredar de `BaseController`.
- Aplicar `[Autorizacion(PermisosScato.Xxx)]` por controller.
- Usar `servicio` (`IServicioRepositorio`) para consultas.
- Usar `servicioComandos` (`IServicioComandos`) para mutaciones.
- Validar `ModelState` y propagar errores de `Resultado` con `ModelState.AgregarErrores(...)`.
- Usar `[DatosUsuario]` cuando se requiera contexto de centro/usuario.

## No hacer
- No incluir lógica de negocio en controller.
- No acceder a `IRepositorio`/`ScatoDbContext` directamente.
- No llamar servicios externos directos desde web.
- No duplicar validaciones de negocio de procesadores.

## Ejemplo mínimo
```csharp
[HttpPost]
[DatosUsuario]
public ActionResult Crear(DatosUsuario datosUsuario, AlmacenDto model)
{
    if (!ModelState.IsValid) return View(model);

    var resultado = servicioComandos.Ejecutar(new CrearAlmacen
    {
        Dto = model,
        Usuario = datosUsuario.NombreUsuario
    });

    if (!resultado.HayErrores) return new AjaxEditSuccessResult();
    ModelState.AgregarErrores(resultado);
    return View(model);
}
```
