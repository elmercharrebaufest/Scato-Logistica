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
    [Autorizacion(PermisosScato.AbmCaracteristicaDeCalidad)]
    public class CaracteristicaDeCalidadPorWorkflowController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public CaracteristicaDeCalidadPorWorkflowController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, datosUsuario,pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, datosUsuario, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, DatosUsuario datosUsuario, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoCaracteristicaDeCalidadPorWorkflow(filtro, paginacion, datosUsuario.CentroId);
        }

        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, int id)
        {
            var tipo = servicio.ObtenerCaracteristicaDeCalidadPorWorkflow(id);
            SetearVista(datosUsuario.CentroId);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, CaracteristicaDeCalidadPorWorkflowDto tipo)
        {
            if (ModelState.IsValid && tipo.CaracteristicasDeCalidadId != null)
            {
                var caracteristicasExistentes = servicio.ListarCaracteristicasDeCalidadPorWorkflow(tipo.MaterialId, datosUsuario.CentroId, tipo.WorkflowId);
                foreach (var id in caracteristicasExistentes)
                {
                    if (tipo.CaracteristicasDeCalidadId.All(x => x != id.CaracteristicaDeCalidadId))
                    {
                        var resultado = servicioComandos.Ejecutar(new EliminarCaracteristicaDeCalidadPorWorkflow { Id = id.Id, Usuario = datosUsuario.NombreUsuario });
                        if (resultado.HayErrores)
                        {
                            ModelState.AgregarErrores(resultado);
                            break;
                        }
                    }
                }
                foreach (var id in tipo.CaracteristicasDeCalidadId)
                {
                    if (caracteristicasExistentes.All(x => x.CaracteristicaDeCalidadId != id))
                    {
                        tipo.CaracteristicaDeCalidadId = id;
                        var resultado = servicioComandos.Ejecutar(new ModificarCaracteristicaDeCalidadPorWorkflow { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                        if (resultado.HayErrores)
                        {
                            ModelState.AgregarErrores(resultado);
                            break;
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    return new AjaxEditSuccessResult();
                }
            }
            SetearVista(datosUsuario.CentroId);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var tipo = servicio.ObtenerCaracteristicaDeCalidadPorWorkflow(id);
            var caracteristicasExistentes = servicio.ListarCaracteristicasDeCalidadPorWorkflow(tipo.MaterialId, datosUsuario.CentroId, tipo.WorkflowId);

            Resultado resultado = null;
            foreach (var idc in caracteristicasExistentes)
            {
                resultado = servicioComandos.Ejecutar(new EliminarCaracteristicaDeCalidadPorWorkflow { Id = idc.Id, Usuario = datosUsuario.NombreUsuario });
                if (resultado.HayErrores)
                {
                    break;
                }
            }
            
            return Content(resultado == null || !resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
        
        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var materialPorWorkflow = new CaracteristicaDeCalidadPorWorkflowDto();
            SetearVista(datosUsuario.CentroId);
            return View(materialPorWorkflow);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, CaracteristicaDeCalidadPorWorkflowDto tipo)
        {
            if (ModelState.IsValid && tipo.CaracteristicasDeCalidadId != null)
            {
                var primera = true;
                foreach (var id in tipo.CaracteristicasDeCalidadId)
                {
                    tipo.CaracteristicaDeCalidadId = id;
                    Resultado resultado;
                    if (primera)
                    {
                        primera = false;
                        resultado = servicioComandos.Ejecutar(new CrearCaracteristicaDeCalidadPorWorkflow { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                    }
                    else
                    {
                        resultado = servicioComandos.Ejecutar(new ModificarCaracteristicaDeCalidadPorWorkflow { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                    }
                    
                    if (resultado.HayErrores)
                    {
                        ModelState.AgregarErrores(resultado);
                        break;
                    }
                }

                if (ModelState.IsValid)
                {
                    return new AjaxEditSuccessResult();
                }
                
            }
            SetearVista(datosUsuario.CentroId);
            return View(tipo);
        }

        [DatosUsuario]
        public JsonResult SetearCaracteristicasCalidadDropDownList(int materialId, DatosUsuario datosUsuario)
        {
            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterial(materialId, datosUsuario.CentroId).OrderBy(x => x.Descripcion).Select(x => new { x.Id, x.Descripcion });

            return Json(caracteristicas, JsonRequestBehavior.AllowGet);
        }

        private void SetearVista(int centroId)
        {
            ViewBag.Workflows = servicio.ListarWorkflowsPorCentro(centroId).Where(w => w.Activo).OrderBy(x => x.Descripcion).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }
    }
}
