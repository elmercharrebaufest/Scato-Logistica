using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AjusteYStockBines)]
    public class AjusteStockBinesController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public AjusteStockBinesController(ILogger log, IServicioComandos servicioComando, IServicioRepositorio servicioRepositorio):base(servicioRepositorio) 
        {

            this.log = log;
            servicioComandos = servicioComando;

        }

        [DatosUsuario]
        public ActionResult Index(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro,datosUsuario.CentroId,pagina,ordenarPor,dirOrden);
            return View();
        }

        private void ListQuery(string filtro, int centroId, int pagina,string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoAjusteYStockBines(filtro, paginacion, centroId);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        public ActionResult Crear()
        {
            var ajuste = new AjusteStockBinesDto { Fecha = DateTime.Today};
            SetearVista();
            return View(ajuste);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(AjusteStockBinesDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearAjusteStockBines { Dto = model });
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

        private void SetearVista()
        {
            var tiposDeBines = servicio.ListarMaterialesBinPallet();
            ViewBag.TiposBinPallet = tiposDeBines.ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s=>s.Descripcion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(AjusteStockBinesDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarAjusteStockBines { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var ajuste = servicio.ObtenerAjusteYStockBines(id);
            SetearVista();
            return View(ajuste);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado =
                servicioComandos.Ejecutar(new EliminarAjusteStockBines
                    {
                        Id = id,
                        NombreUsuario = datosUsuario.NombreUsuario
                    });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
