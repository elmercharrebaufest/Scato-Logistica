using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmActividadPorDispositivo)]
    public class ActividadPorDispositivoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioWorkflows servicioWorkflows;

        public ActividadPorDispositivoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador, IServicioWorkflows servicioWorkflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.servicioWorkflows = servicioWorkflows;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var puestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId);
            ViewBag.PuestosDeTrabajo = puestosDeTrabajo.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto, datosUsuario.PuestoDeTrabajoId.ToString(CultureInfo.InvariantCulture));
            ViewBag.Items = new ListaPaginada<ActividadPorDispositivoDto>(new List<ActividadPorDispositivoDto>(), 1, 1,0);
            if (puestosDeTrabajo.Count == 0)
            {
                ModelState.AddModelError("", Textos.ActividadPorDispositivo_ErrorSinPuesto);
            }
            var puesto = datosUsuario.PuestoDeTrabajoId != 0
                             ? datosUsuario.PuestoDeTrabajoId
                             : puestosDeTrabajo.Count != 0 ? puestosDeTrabajo.First().Id : 0;
            return View(new ActividadPorDispositivoDto { PuestoDeTrabajoId = puesto });
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(int PuestoDeTrabajoId, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(PuestoDeTrabajoId, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar", new ActividadPorDispositivoDto { PuestoDeTrabajoId = PuestoDeTrabajoId });
        }

        private void ListQuery(int puestoId, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoActividadesPorBarreraSemaforo(puestoId, paginacion);
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(centroId).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario, int puestoId)
        {
            var puesto = servicio.ObtenerPuestoDeTrabajo(puestoId)?? new PuestoDeTrabajoDto();
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(datosUsuario.CentroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion);
            ViewBag.Barreras = servicioOrquestador.ListarBarrerasSemaforos().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Camaras = servicioOrquestador.ListarCamaras().ToSelectList(x => x.Codigo, x => x.Descripcion);
            if (puestoId == 0)
            {
                ModelState.AddModelError("PuestoDeTrabajoDescripcion", string.Format(Textos.Error_Requerido, Textos.PuestoDeTrabajo));
            }
            return View(new ActividadPorDispositivoDto { PuestoDeTrabajoId = puestoId, PuestoDeTrabajoDescripcion = puesto.NombrePuesto });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ActividadPorDispositivoDto model, string salidas, string videoCamarasJson, DatosUsuario datosUsuario,string entradas)
        {
            if (!string.IsNullOrEmpty(videoCamarasJson))
            {
                model.VideoCamaras = videoCamarasJson.FromJson<VideoCamaraDto[]>().ToList();
            }

            if (ModelState.IsValid && salidas != "" && entradas != "")
            {
                model.Salida = string.Join(",", salidas.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
                model.Entrada = string.Join(",", entradas.FromJson<EntradaDto[]>().Select(x => x.Codigo.Trim()));
                var resultado = servicioComandos.Ejecutar(new CrearActividadPorDispositivo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            else if(ModelState.IsValid && salidas != "")
            {
                model.Salida = string.Join(",", salidas.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
                model.Entrada = string.IsNullOrEmpty(entradas) ? " " : string.Join(",", entradas.FromJson<EntradaDto[]>().Select(x => x.Codigo.Trim()));
                var resultado = servicioComandos.Ejecutar(new CrearActividadPorDispositivo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            if (string.IsNullOrEmpty(salidas))
            {
                ModelState.AddModelError("Salida", string.Format(Textos.Error_Requerido, Textos.Salida));
            }
            //if (string.IsNullOrEmpty(entradas))
            //{
            //    ModelState.AddModelError("Entrada", string.Format(Textos.Error_Requerido, Textos.Entrada));
            //}

            SetearVista(model, datosUsuario.CentroId);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var aModificar = servicio.ObtenerActividadPorDispositivo(id);
            SetearVista(aModificar, datosUsuario.CentroId);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ActividadPorDispositivoDto model, string salidas, string videoCamarasJson, DatosUsuario datosUsuario, string entradas)
        {
            if (!string.IsNullOrEmpty(videoCamarasJson))
            {
                model.VideoCamaras = videoCamarasJson.FromJson<VideoCamaraDto[]>().ToList();
            }

            if (ModelState.IsValid && salidas != "" && entradas != "")
            {
                model.Salida = string.Join(",", salidas.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
                model.Entrada = string.Join(",", entradas.FromJson<EntradaDto[]>().Select(x => x.Codigo.Trim()));
                var resultado = servicioComandos.Ejecutar(new ModificarActividadPorDispositivo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            else if (ModelState.IsValid && salidas != "")
            {
                model.Salida = string.Join(",", salidas.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
                model.Entrada = string.IsNullOrEmpty(entradas) ? null : string.Join(",", entradas.FromJson<EntradaDto[]>().Select(x => x.Codigo.Trim()));
                var resultado = servicioComandos.Ejecutar(new ModificarActividadPorDispositivo { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            if (string.IsNullOrEmpty(salidas))
            {
                ModelState.AddModelError("Salida", string.Format(Textos.Error_Requerido, Textos.Salida));
            }
            //if (string.IsNullOrEmpty(entradas))
            //{
            //    ModelState.AddModelError("Entrada", string.Format(Textos.Error_Requerido, Textos.Entrada));
            //}
            SetearVista(model, datosUsuario.CentroId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarActividadPorDispositivo { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        private void SetearVista(ActividadPorDispositivoDto dto, int centroId)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(centroId).OrderBy(f => f.Descripcion).ToSelectList(s => s.Id.ToString(CultureInfo.InvariantCulture), s => s.Descripcion, dto.WorkflowId.ToString(CultureInfo.InvariantCulture));
            if (dto.WorkflowId > 0)
            {
                var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(dto.WorkflowId);
                ViewBag.Actividades = actividades.ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x) ?? x, dto.Actividad);
            }
            ViewBag.Barreras = servicioOrquestador.ListarBarrerasSemaforos().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Camaras = servicioOrquestador.ListarCamaras().ToSelectList(x => x.Codigo, x => x.Descripcion);
        }

        public ActionResult ObtenerActividades(int workflowId)
        {
            var actividades = servicioWorkflows.ListarActividadesPorDefinicionWorkflow(workflowId);

            return Json(
                    actividades.Select(x => new { value = x, text = Textos.ResourceManager.GetString("Act" + x) ?? x }).OrderBy(x => x.text),
                    JsonRequestBehavior.AllowGet
                );
        }

        public JsonResult ObtenerDispositivos(int id)
        {
            var dispositivos = servicioOrquestador.ListarBarrerasSemaforos();
            var result = servicio.ObtenerActividadPorDispositivo(id);

            if (result != null)
            {
                var salidasConfig = result.Salida.Split(',');
                var salidas = dispositivos
                    .Where(d => salidasConfig.Contains(d.Codigo))
                    .Select(d => new {d.Codigo, d.Descripcion});
                return Json(salidas, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerDispositivosEntrada(int id)
        {
            var dispositivo = servicioOrquestador.ListarBarrerasSemaforos();
            var respuesta = servicio.ObtenerActividadPorDispositivo(id);
            
            if (respuesta != null)
            {
                var entradasConfig = respuesta.Entrada.Split(',');
                var entradas = dispositivo
                    .Where(d => entradasConfig.Contains(d.Codigo))
                    .Select(d => new { d.Codigo, d.Descripcion });
                return Json(entradas, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
