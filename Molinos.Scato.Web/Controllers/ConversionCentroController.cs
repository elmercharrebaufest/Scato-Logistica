using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.TablaConversionCentro)]
    public class ConversionCentroController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        public ConversionCentroController(ILogger log,IServicioRepositorio servicio, IServicioComandos servicioComandos) : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [HttpGet]
        public ActionResult Index(int? camaraId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaCentro(camaraId,pagina,ordenarPor,dirOrden);
            SetearVista();
            return View(new ConversionCentroDto{CamaraId = camaraId ?? 0});
        }

        [HttpGet]
        public ActionResult Indexa(int? camaraId, int pagina = 1, string ordenarPor = "Id",
                                   DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsultaCentro(camaraId,pagina,ordenarPor,dirOrden);
            SetearVista();
            return View("ListarConversionCentros");
        }

        private void ListarConsultaCentro(int? camaraId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoConversionCentro(camaraId, paginacion);
        }

        private void SetearVista()
        {
            ViewBag.Camaras = servicio.ListarCamaras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            
            ViewBag.Centros = servicio.ListarCentros().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [HttpPost]
        public ActionResult Crear(ConversionCentroDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConversionCentro{Dto = tipo});
                if (resultado.HayErrores)
                {
                    ModelState.AgregarErrores(resultado);
                }
            }
            SetearVista();
            ListarConsultaCentro(tipo.CamaraId,1,"Id",DirOrden.Asc);
            return View("Index", tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConversionCentro{ Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
