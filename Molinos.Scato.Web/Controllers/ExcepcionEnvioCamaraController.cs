using System.Collections.Generic;
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
    [Autorizacion(PermisosScato.AbmExcepcionEnvioCamara)]
    public class ExcepcionEnvioCamaraController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ExcepcionEnvioCamaraController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "MaterialId", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "MaterialId", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro,pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoExcepcionEnvioCamara(filtro, paginacion);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var tipo = servicio.ObtenerExcepcionEnvioCamara(id);
            SetearVista();
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(ExcepcionEnvioCamaraDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid && tipo.CaracteristicasDeCalidadId != null)
            {
                var caracteristicasExistentes = servicio.ListarExcepcionEnvioCamara(tipo.MaterialId, tipo.TipoComercialId, tipo.ProveedorId, tipo.EntregadorId);
                foreach (var id in caracteristicasExistentes)
                {
                    if (tipo.CaracteristicasDeCalidadId.All(x => x != id.CaracteristicaId))
                    {
                        var resultado = servicioComandos.Ejecutar(new EliminarExcepcionEnvioCamara { Id = id.Id, Usuario = datosUsuario.NombreUsuario });
                        if (resultado.HayErrores)
                        {
                            ModelState.AgregarErrores(resultado);
                            break;
                        }
                    }
                }
                foreach (var id in tipo.CaracteristicasDeCalidadId)
                {
                    if (caracteristicasExistentes.All(x => x.CaracteristicaId != id))
                    {
                        tipo.CaracteristicaId = id;
                        var resultado = servicioComandos.Ejecutar(new ModificarExcepcionEnvioCamara { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
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
            SetearVista();
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarExcepcionEnvioCamara { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            SetearVista();
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(ExcepcionEnvioCamaraDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid && tipo.CaracteristicasDeCalidadId != null)
            {
                var primera = true;
                foreach (var id in tipo.CaracteristicasDeCalidadId)
                {
                    tipo.CaracteristicaId = id;
                    Resultado resultado;
                    if (primera)
                    {
                        primera = false;
                        resultado = servicioComandos.Ejecutar(new CrearExcepcionEnvioCamara { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
                    }
                    else
                    {
                        resultado = servicioComandos.Ejecutar(new ModificarExcepcionEnvioCamara { Dto = tipo, Usuario = datosUsuario.NombreUsuario });
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
            SetearVista();
            return View(tipo);
        }

        private void SetearVista()
        {
            ViewBag.TiposComerciales = servicio.ListarTiposComerciales().ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [DatosUsuario]
        public JsonResult ObtenerCaracteristicas(int materialId, DatosUsuario datosUsuario)
        {
            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterial(materialId, datosUsuario.CentroId).OrderBy(x => x.Descripcion).Select(x => new { x.Id, x.Descripcion });

            return Json(caracteristicas, JsonRequestBehavior.AllowGet);
        }
    }
}
