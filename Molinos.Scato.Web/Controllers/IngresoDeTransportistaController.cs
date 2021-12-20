using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresoDeTransportista)]
    public class IngresoDeTransportistaController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoDeTransportistaService> factory;
        private ILogger log;

        public IngresoDeTransportistaController(ILogger log, IServicioActividadFactory<IIngresoDeTransportistaService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            SetearVista(id);

            return View();
        }

        private void SetearVista(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowInstance = id;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            CargarProvincias();
            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(id);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(TransportistaDto model, string workflow, int workflowDefinicionId, Guid workflowInstance, string numeroDocumentoIngreso, DatosUsuario datosUsuario)
        {
            ModelState["Id"] = new ModelState { Value = new ValueProviderResult(0, "0",CultureInfo.CurrentCulture) };
            if (ModelState.IsValid)
            {
                var service = factory.CrearServicio(workflowDefinicionId);
                var recorridos = servicio.ListarRecorridoPorNumeroDocumentoYWorkflow(numeroDocumentoIngreso, workflowDefinicionId);
                var resultado = new Resultado();
                if (recorridos.Count == 0)
                {
                    resultado.Errores.Add("", Textos.Error_Generico);
                }
                foreach (var recorridoDto in recorridos)
                {
                    var controlRecorrido = new ControlRecorridoDto {Actividad = Textos.ActIngresoDeTransportista, ActividadXaml = "IngresoDeTransportista", WorkflowInstanceId = workflowInstance, PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId, NombreUsuario = datosUsuario.NombreUsuario};
                    resultado = service.IngresoDeTransportista(model, recorridoDto.InstanciaWorkflow, controlRecorrido);
                    if (resultado.HayErrores)
                    {
                        ModelState.AgregarErrores(resultado);
                        break;
                    }
                }
                if (!resultado.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            ViewBag.Workflow = workflow;
            ViewBag.WorkflowInstance = workflowInstance;
            ViewBag.NumeroDocumentoIngreso = numeroDocumentoIngreso;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            CargarProvincias(model);
            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(workflowInstance);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
            return View(model);
        }

        [ActionName("CargarLocalidades")]
        public JsonResult CargarLocalidades(int? provinciaId)
        {
            if (provinciaId != 0 && provinciaId != null)
            {
                var localidades =
                    servicio.ListarLocalidadesPorProvincia((int)provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<SelectList>(), JsonRequestBehavior.AllowGet);
        }

        private void CargarProvincias(TransportistaDto model = null)
        {
            var provincias = servicio.ListarProvincias();

            int provinciaId = 0;
            if (model != null && model.ProvinciaId != null)
            {
                provinciaId = model.ProvinciaId.Value;
            }

            ViewBag.Provincias = provincias.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.Localidades = servicio.ListarLocalidadesPorProvincia(provinciaId)
                            .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
        }
    }
}
