using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresoBinSalida)]
    public class IngresoBinSalidaController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoBinSalidaService> factory;
        private ILogger log;

        public IngresoBinSalidaController(ILogger log, IServicioActividadFactory<IIngresoBinSalidaService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            SetearVista(id, datosUsuario);

            return View();
        }

        private void SetearVista(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var remitoId = servicio.ObtenerRemitoBodegaUvaIdPorGuid(id);
            var tipoBin = servicio.ListarMaterialesBin(recorrido.WorkflowId, datosUsuario.CentroId, ClaseBin.Bines);
            var binesIngreso = servicio.ObtenerDescargasDeBinesPorRemitoBodegaUva(remitoId);
            
            ViewBag.RemitoId = remitoId;
            ViewBag.TipoBinJson = tipoBin.ToJson();
            ViewBag.BinesIngreso = binesIngreso;
            ViewBag.KilosBinesIngreso = binesIngreso.Sum(x => x.CantidadBines*x.Peso);
            ViewBag.Workflow = recorrido.WorkflowCodigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Guid = id;
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Index(string binesSalida, string observacion, string workflow, int workflowDefinicionId, Guid guid, DatosUsuario datosUsuario)
        {
            var binesCargados = binesSalida.FromJson<CargaDeBinesDto[]>();
           
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = guid,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActIngresoBinSalida,
                ActividadXaml = "IngresoBinSalida",
                Decision = false
            };

            var serivce = factory.CrearServicio(workflowDefinicionId);
            var resultado = serivce.IngresoBinSalida(binesCargados, guid, controlRecorrido, observacion);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            TempData["Alerta"] = Textos.IngresoBinSalida_Error;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return RedirectToAction("Index", new { id = guid });
        }
        
    }
}
