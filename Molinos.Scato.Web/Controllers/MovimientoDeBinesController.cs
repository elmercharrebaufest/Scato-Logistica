using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class MovimientoDeBinesController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public MovimientoDeBinesController(ILogger log, IServicioComandos servicioComando, IServicioRepositorio servicioRepositorio):base(servicioRepositorio) 
        {

            this.log = log;
            servicioComandos = servicioComando;

        }

        [DatosUsuario]
        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Index(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro,datosUsuario.CentroId,pagina,ordenarPor,dirOrden);
            return View();
        }

        private void ListQuery(string filtro, int centroId, int pagina,string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoMovimientoDeBines(filtro, paginacion, centroId);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Listar(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Crear()
        {
            SetearVista();
            return View(new MovimientoDeBinesDto { Fecha = DateTime.Now, Movimiento = null, TipoAjuste = null});
        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Crear(MovimientoDeBinesDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.NombreUsuario = datosUsuario.NombreUsuario;
                model.CentroOrigenId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearMovimientoDeBines { Dto = model});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }

                ModelState.AgregarErrores(resultado);
                SetearVista();
                return View(model);
            }
            SetearVista();
            return View(model);
        }

        [Autorizacion(PermisosScato.WorkflowMovimientoDeBines)]
        [DatosUsuario]
        public ActionResult CrearWorkflow(DatosUsuario datosUsuario)
        {
            SetearVista();
            return View(new MovimientoDeBinesDto { Fecha = DateTime.Now, Movimiento = null, TipoAjuste = null,CentroId = datosUsuario.CentroId, CentroDescripcion = datosUsuario.CentroDescripcion});
        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.WorkflowMovimientoDeBines)]
        public ActionResult CrearWorkflow(MovimientoDeBinesDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.NombreUsuario = datosUsuario.NombreUsuario;
                model.CentroOrigenId = datosUsuario.CentroId;
                if (model.TipoAjuste.HasValue && (model.TipoAjuste.Value == TipoStockBines.Productor || model.TipoAjuste.Value == TipoStockBines.VinedoPropio))
                {
                    model.Movimiento = model.Movimiento == TipoDeWorkflow.Egreso ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso;
                }
                var resultado = servicioComandos.Ejecutar(new CrearMovimientoDeBines { Dto = model});
                if (!resultado.HayErrores && model.TipoAjuste.HasValue && (model.TipoAjuste.Value == TipoStockBines.Productor || model.TipoAjuste.Value == TipoStockBines.VinedoPropio))
                {
                    model.Movimiento = model.Movimiento == TipoDeWorkflow.Egreso ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso;
                    model.TipoAjuste = TipoStockBines.Centro;
                    model.CentroId = datosUsuario.CentroId;

                    servicioComandos.Ejecutar(new CrearMovimientoDeBines { Dto = model});
                }
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("WorkflowCreado", new { ((ResultadoCrear)resultado).Id });
                }

                ModelState.AgregarErrores(resultado);
                SetearVista();
                return View(model);
            }
            SetearVista();
            return View(model);
        }

        public ActionResult WorkflowCreado(int id)
        {
            var objeto = servicio.ObtenerMovimientoDeBines(id);
            var dto = new WorkflowCreadoDto
                {
                    Cantidad = objeto.Cantidad,
                    TipoAjuste = objeto.TipoAjuste,
                    Movimiento = objeto.Movimiento,
                    CentroDescripcion = objeto.CentroOrigenDescripcion,
                    Numero = objeto.Numero,
                    MaterialDescripcion = objeto.MaterialDescripcion,
                    ProveedorDescripcion = objeto.ProveedorDescripcion,
                    VinedoPropioDescripcion = objeto.VinedoPropioDescripcion,
                    StockCentro = servicio.ObtenerStock(TipoStockBines.Centro, DateTime.Now, objeto.TipoAjuste == TipoStockBines.Centro ? objeto.CentroId ?? 0 : objeto.CentroOrigenId, objeto.MaterialId),
                    StockProveedor = objeto.ProveedorId.HasValue && objeto.ProveedorId > 0 ? servicio.ObtenerStock(TipoStockBines.Productor, DateTime.Now, objeto.ProveedorId ?? 0, objeto.MaterialId) : 0,
                    StockVinedoPropio = objeto.VinedoPropioId.HasValue && objeto.VinedoPropioId > 0 ? servicio.ObtenerStock(TipoStockBines.VinedoPropio, DateTime.Now, objeto.VinedoPropioId ?? 0, objeto.MaterialId) : 0
                };
            return View(dto);
        }

        private void SetearVista()
        {
            var tiposDeBines = servicio.ListarMaterialesBinPallet();
            ViewBag.TiposBinPallet = tiposDeBines.ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s=>s.Descripcion);
        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Modificar(MovimientoDeBinesDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.NombreUsuario = datosUsuario.NombreUsuario;
                var resultado = servicioComandos.Ejecutar(new ModificarMovimientoDeBines { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Modificar(int id)
        {
            var ajuste = servicio.ObtenerMovimientoDeBines(id);
            SetearVista();
            return View(ajuste);

        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.MovimientoDeBines)]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado =
                servicioComandos.Ejecutar(new EliminarMovimientoDeBines
                    {
                        Id = id,
                        NombreUsuario = datosUsuario.NombreUsuario
                    });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
