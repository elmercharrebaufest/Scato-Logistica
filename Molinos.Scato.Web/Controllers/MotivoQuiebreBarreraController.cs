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
    [Autorizacion(PermisosScato.MotivoQuiebreBarrera)]
    public class MotivoQuiebreBarreraController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public MotivoQuiebreBarreraController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int? puestoId, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            SetearDatos(datosUsuario.CentroId);
            ListarConsulta(puestoId, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(int? puestoId, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            ListarConsulta(puestoId, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(int? puestoId, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoMotivoQuiebreBarrera(puestoId ?? 0, centroId, paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerMotivoQuiebreBarrera(id);
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Modificar(MotivoQuiebreBarreraDto tipo)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarMotivoQuiebreBarrera { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(tipo);
        }

        private void SetearDatos(int centroId)
        {
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(centroId).OrderBy(c => c.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
        }
    }
}
