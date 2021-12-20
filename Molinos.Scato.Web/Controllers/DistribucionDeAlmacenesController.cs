using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    [Autorizacion(PermisosScato.ActividadDistribucionDeAlmacenes)]
    public class DistribucionDeAlmacenesController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioActividadFactory<IDistribucionDeAlmacenesService> factory;

        public DistribucionDeAlmacenesController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IDistribucionDeAlmacenesService> factory)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario,Guid id)
        {
            return View(SetearVista(datosUsuario, id));
        }

        private DistribucionDeAlmacenesDto SetearVista(DatosUsuario datosUsuario, Guid id, string json = null)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var model = new DistribucionDeAlmacenesDto
            {
                Centro = recorrido.Centro.Descripcion,
                Fecha = DateTime.Now,
                InstanceId = recorrido.InstanciaWorkflow,
                NombreDeUsuario = datosUsuario.NombreUsuario,
                Patente = recorrido.Patente,
                PesoNetoBodegaEnLitros = recorrido.PesoNetoBodegaEnLitros.HasValue ? recorrido.PesoNetoBodegaEnLitros.Value : 0,
                Workflow = recorrido.Workflow.Descripcion,
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                MaterialId = recorrido.Material.Id,
                NumeroDeDocumentoDeIngreso = recorrido.NumeroDocumentoIngreso,
            };
            ViewBag.DistribucionesDeAlmacenesJsonPostBack = json;
            ViewBag.Almacenes = servicio.ListarAlmacenesPorMaterialYCentro(datosUsuario.CentroId, recorrido.Material.Id,false).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return model;
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Index(DatosUsuario datosUsuario, DistribucionDeAlmacenesDto model)
        {
            if (ModelState.IsValid)
            {
                model.DistribucionesDeAlmacenes = model.DistribucionesDeAlmacenesJson.FromJson<DistribucionDeAlmacenDto[]>().ToList();

                if (model.PesoNetoBodegaEnLitros < model.DistribucionesDeAlmacenes.Sum(x => x.Litros))
                {
                    var r = new Resultado();
                    r.Errores.Add("litros", Textos.Error_LitrosInvalidos);
                    ModelState.AgregarErrores(r);
                }
                else if (model.PesoNetoBodegaEnLitros > model.DistribucionesDeAlmacenes.Sum(x => x.Litros))
                {
                    var r = new Resultado();
                    r.Errores.Add("litros", Textos.Error_LitrosDisponibles);
                    ModelState.AgregarErrores(r);
                }
                else
                {
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActDistribucionDeAlmacenes,
                        ActividadXaml = "DistribucionDeAlmacenes",
                        NombreUsuario = datosUsuario.NombreUsuario,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        WorkflowInstanceId = model.InstanceId,
                        Fecha = DateTime.Now
                    };
                    model.Fecha = DateTime.Now;
                    var actividad = factory.CrearServicio(model.WorkflowDefinicionId);
                    var resultado = actividad.DistribucionDeAlmacenes(model.InstanceId, controlRecorrido, model);

                    if (!resultado.HayErrores)
                    {
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                    ModelState.AgregarErrores(resultado);
                }
            }
            model = SetearVista(datosUsuario, model.InstanceId, model.DistribucionesDeAlmacenesJson);
            return View(model);
        }
    }
}
