using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmBorradoDeDocumento)]
    public class BorradoDeDocumentoController : BaseController
    {
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public BorradoDeDocumentoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, FiltroRecorridoModel filtro, string ordenarPor = "NumeroDocumentoIngreso", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(new FiltroRecorridoModel { NumeroDocumentoIngreso = "0" }, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroRecorridoModel filtro, string ordenarPor = "NumeroDocumentoIngreso", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(FiltroRecorridoModel filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(filtro.TipoDocumentoIngreso, filtro.NumeroDocumentoIngreso, filtro.Patente, "", centroId, paginacion);
        }

        [DatosUsuario]
        [HttpPost]
        [Autorizacion(PermisosScato.BorrarDocumentosTerminados)]
        public ActionResult EliminarTerminado(DatosUsuario datosUsuario, int id, string motivo)
        {
            return Eliminar(datosUsuario, id, motivo);
        }

        [DatosUsuario]
        [HttpPost]
        [Autorizacion(PermisosScato.BorrarDocumentosNoTerminados)]
        public ActionResult EliminarNoTerminado(DatosUsuario datosUsuario, int id, string motivo)
        {
            return Eliminar(datosUsuario, id, motivo);
        }

        private ActionResult Eliminar(DatosUsuario datosUsuario, int id, string motivo)
        {
            var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorRecorridoId(id);
            var resultado = servicioComandos.Ejecutar(new EliminarRecorrido { Id = id, Motivo = motivo, NombreUsuario = datosUsuario.NombreUsuario }) as ResultadoEliminarRecorrido;
            if (resultado != null && resultado.HayErrores)
            {
                return Json(new { eliminado = resultado.Errores.Values.First() }, JsonRequestBehavior.AllowGet);
            }
            var texto = resultado != null ? resultado.Advertencias.Values.Aggregate("", (current, advertencia) => current + advertencia + ". ") : "";

            if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
            {
                var resultadoWf = workflows.EliminarInstanciaWorkflow(instanciaWorkflow);
                if (resultadoWf != null && resultadoWf.HayErrores)
                {
                    return Json(new { eliminado = resultadoWf.Errores.Values.First(), advertencia = texto }, JsonRequestBehavior.AllowGet);
                }
            }

            var pagos = BuscarPagosTasaMunicipalPorInstanciaDeWorkflow(instanciaWorkflow);

            if (pagos.Any())
            {
                foreach (var pago in pagos)
                {
                    servicioComandos.Ejecutar(new LiberarPagoTasaMunicipal
                    {
                        PagoId = pago.Id,
                        InstanceId = instanciaWorkflow
                    });
                }
            }
            return Json(new { eliminado = "true", advertencia = texto }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PagosTasaMunicipal> BuscarPagosTasaMunicipalPorInstanciaDeWorkflow(Guid instanciaWorkflow)
        {
            var pagos = servicio.ObtenerPagosDigitalesPorInstanceId(instanciaWorkflow);
            return pagos;
        }  
    }
}
