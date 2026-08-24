---
applyTo: "**/Molinos.Scato.WebPuertoApi/Controllers/*.cs"
---
## Objetivo
Implementar Web API 2 de puerto con contratos HTTP explícitos, permisos y routing por atributo.
## Hacer
- Heredar de `BaseController`
- Retornar `HttpResponseMessage` con `Request.CreateResponse(...)`
- Declarar `[Route("api/{Entidad}/{Accion}")]` en cada acción
- Proteger con `[Autorizacion(PermisosScato.X)]`
- Inyectar servicios por constructor (sin instanciación manual)
- Obtener `workflow` desde `servicio.ObtenerUltimaWorkflowDefinicionPorCodigo(...)` antes de ejecutar
- Validar request y devolver respuesta coherente (`BadRequest`, `OK`, `InternalServerError`) sin exponer detalles internos
## No hacer
- No devolver primitivos ni entidades de dominio directas
- No usar `new` para construir servicios
- No mezclar múltiples dominios de puerto en un mismo controller
- No envolver toda la acción en `try/catch (Exception)` como patrón por defecto
- No exponer `Exception.Message` ni stacktrace al cliente
## Ejemplo mínimo
```csharp
[Autorizacion(PermisosScato.LineUp)]
public class MiEntidadController : BaseController
{
    private readonly IServicioComandos comandos;

    public MiEntidadController(IServicioRepositorio servicio, IServicioComandos comandos) : base(servicio)
    {
        this.comandos = comandos;
    }

    [HttpPost, Route("api/MiEntidad/Crear"), Autorizacion(PermisosScato.LineUp)]
    public HttpResponseMessage Crear(MiEntidadDto dto)
    {
        if (dto == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
        var resultado = comandos.Ejecutar(new CrearMiEntidad { Dto = dto });
        return resultado.HayErrores
            ? Request.CreateResponse(HttpStatusCode.BadRequest, resultado.Errores)
            : Request.CreateResponse(HttpStatusCode.OK);
    }
}
```
