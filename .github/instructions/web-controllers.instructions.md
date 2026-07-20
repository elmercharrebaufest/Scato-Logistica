---
applyTo: "**/Molinos.Scato.Web/Controllers/*.cs"
---

# Reglas de la capa Web — Controllers

## Principio fundamental
Los controllers reciben la request HTTP, delegan consultas a `IServicioRepositorio` y mutaciones a `IServicioComandos`, y devuelven la vista o resultado JSON. No contienen lógica de negocio.

## Herencia obligatoria

Todo controller hereda de `BaseController`:

```csharp
namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmNuevoConcepto)]
    public class NuevoConceptoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public NuevoConceptoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
    }
}
```

`BaseController` provee la propiedad `servicio` (`IServicioRepositorio`) y maneja internacionalización, cookies de sesión (`CentroId`, `BalanzaId`, etc.).

## Autorización

Todo controller lleva `[Autorizacion(PermisosScato.XxxPermiso)]`. Agregar el permiso en `PermisosScato` (en `Molinos.Scato.Dominio.Seguridad`) antes de crear el controller.

## Patrón de acciones CRUD

```csharp
// GET lista completa (primera carga de la página)
[DatosUsuario]
public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
{
    var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
    ViewBag.Items = servicio.ListarPaginadoNuevoConcepto(datosUsuario.CentroId, paginacion);
    return View();
}

// GET lista parcial para recargas AJAX
[AjaxOnly]
[ActionName("Index")]
[DatosUsuario]
public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
{
    var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
    ViewBag.Items = servicio.ListarPaginadoNuevoConcepto(datosUsuario.CentroId, paginacion);
    return View("Listar");
}

// GET formulario de creación
[DatosUsuario]
public ActionResult Crear()
{
    ViewBag.Centros = servicio.ListarCentros().ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
    return View();
}

// POST creación
[DatosUsuario]
[HttpPost]
public ActionResult Crear(DatosUsuario datosUsuario, NuevoConceptoDto model)
{
    if (ModelState.IsValid)
    {
        var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearNuevoConcepto
        {
            Dto = model,
            Usuario = datosUsuario.NombreUsuario
        });

        if (!resultado.HayErrores)
        {
            return new AjaxEditSuccessResult();
        }
        ModelState.AgregarErrores(resultado);
    }
    return View(model);
}

// GET formulario de edición
[DatosUsuario]
public ActionResult Modificar(int id)
{
    var model = servicio.ObtenerNuevoConcepto(id);
    return View(model);
}

// POST edición
[DatosUsuario]
[HttpPost]
public ActionResult Modificar(DatosUsuario datosUsuario, NuevoConceptoDto model)
{
    if (ModelState.IsValid)
    {
        var resultado = servicioComandos.Ejecutar(new ModificarNuevoConcepto
        {
            Dto = model,
            Usuario = datosUsuario.NombreUsuario
        });

        if (!resultado.HayErrores)
        {
            return new AjaxEditSuccessResult();
        }
        ModelState.AgregarErrores(resultado);
    }
    return View(model);
}

// POST eliminación
[DatosUsuario]
[HttpPost]
public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
{
    var resultado = servicioComandos.Ejecutar(new EliminarNuevoConcepto { Id = id, Usuario = datosUsuario.NombreUsuario });
    return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
}
```

## Atributo `[DatosUsuario]`

Usar en acciones que necesiten el contexto del usuario (`CentroId`, `NombreUsuario`, `BalanzaId`, etc.). El modelo binding inyecta un `DatosUsuario` como parámetro de la acción.

## Errores de comando en la vista

```csharp
// Propaga los errores del Resultado al ModelState de MVC para mostrarlos en el formulario
ModelState.AgregarErrores(resultado);
```

## Consultas con `servicio` (`IServicioRepositorio`)

Usar `servicio` para todas las consultas de datos (listas para dropdowns, datos del formulario de edición, listados paginados). No llamar `IServicioComandos` para queries.

## Mutaciones con `servicioComandos` (`IServicioComandos`)

Todas las operaciones que modifican datos van a través de `servicioComandos.Ejecutar(new XxxComando { ... })`. El resultado siempre tiene `HayErrores` que debe verificarse.

## Casting de ResultadoCrear

Para operaciones de creación donde se necesita el `Id` generado:
```csharp
var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearNuevoConcepto { ... });
if (!resultado.HayErrores)
{
    // resultado.Id contiene el Id de la entidad creada
}
```

## Vistas y AJAX

- Acción `Index` (GET sin `[AjaxOnly]`): retorna la página completa con `return View()`.
- Acción `Listar` (con `[AjaxOnly]` y `[ActionName("Index")]`): retorna solo la tabla parcial con `return View("Listar")`.
- Las vistas parciales de listado se cargan vía AJAX desde la vista principal.

## Retornos JSON

Para acciones que retornan datos a llamadas AJAX de lectura:
```csharp
public JsonResult ObtenerDatos(int id)
{
    var datos = servicio.ObtenerDatos(id);
    return Json(datos, JsonRequestBehavior.AllowGet);
}
```

## Prohibido en controllers
- Lógica de negocio que debería estar en un procesador.
- Acceso directo a `IRepositorio` o `ScatoDbContext`.
- Llamadas a servicios externos (AFIP, SAP) directamente.
- Validaciones de negocio (unicidad, existencia) — eso va en `Validar()` del procesador.
- Validaciones de formato más allá de `ModelState.IsValid` (las Data Annotations del DTO ya cubren eso).
