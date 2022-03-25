using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
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
    [Autorizacion(PermisosScato.AjusteDeStock)]
    public class AjusteDeStockController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public AjusteDeStockController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(string filtro, DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoAjusteDeStock(filtro, paginacion, centroId);
        }

        public ActionResult Crear()
        {
            var ajuste = new AjusteDeStockDto { Fecha = DateTime.Today, PesoBrutoIngreso = 0, PesoNetoEgreso = 0, PesoNetoIngreso = 0 };
            SetearVista();
            return View(ajuste);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(AjusteDeStockDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                model.NombreUsuario = datosUsuario.NombreUsuario;
                model.NumeroDocumentoIngreso = model.NumeroDocumentoIngreso.Replace("-", " ");                    

                if ((model.PesoBrutoIngreso == 0 && model.PesoNetoIngreso != 0 && model.PesoNetoEgreso != 0) || (model.PesoBrutoIngreso != 0 && model.PesoNetoIngreso == 0 && model.PesoNetoEgreso != 0) || (model.PesoBrutoIngreso != 0 && model.PesoNetoIngreso != 0 && model.PesoNetoEgreso == 0) || (model.PesoBrutoIngreso != 0 && model.PesoNetoIngreso != 0 && model.PesoNetoEgreso != 0))
                {
                    SetearVista();
                    ModelState.AddModelError("PesoBrutoIngreso", Textos.AjusteDeStock_MasDeUnPeso);
                    SetearVista();
                    return View(model);
                }

                var resultado = servicioComandos.Ejecutar(new CrearAjusteDeStock { Dto = model, Usuario = datosUsuario.NombreUsuario});
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

        public ActionResult Modificar(int id)
        {
            var ajuste = servicio.ObtenerAjusteDeStock(id);
            SetearVista();
            return View(ajuste);

        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(AjusteDeStockDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.NombreUsuario = datosUsuario.NombreUsuario;
                model.NumeroDocumentoIngreso = model.NumeroDocumentoIngreso.Replace("-", " ");
                var resultado = servicioComandos.Ejecutar(new ModificarAjusteDeStock { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarAjusteDeStock { Id = id, NombreUsuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearVista()
        {
            var tiposComprobante = new List<SelectListItem>
                {
                    new SelectListItem {Selected = true, Value = "", Text = ""}
                };

            var tipos = servicio.ListarTiposComprobantesOncca().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.CodigoOncca + " - " + f.Descripcion);
            tiposComprobante.AddRange(tipos);
            ViewBag.TiposComprobante = tiposComprobante;
        } 
    }
}
