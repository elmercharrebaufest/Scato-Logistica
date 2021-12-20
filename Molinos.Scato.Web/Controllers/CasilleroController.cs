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
    [Autorizacion(PermisosScato.AbmCasillero)]
    public class CasilleroController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public CasilleroController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }
        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }


        private void ListQuery(int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoCasilleros(paginacion, centroId);
        }

        public ActionResult Crear()
        {
            ViewBag.Eliminar = "false";
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, CasilleroModel model)
        {
            if (ModelState.IsValid)
            {
                model.Casillero.CentroId = datosUsuario.CentroId;
                var casillerosACrear = new List<CasilleroDto>();
                if (model.Masivo)
                {
                    if (model.Casillero.Numero.Substring(0, 4) != model.Hasta.Substring(0, 4))
                    {
                        ModelState.AddModelError("Hasta", Textos.Casillero_PrefijosDistintos);
                    }
                    if (ModelState.IsValid)
                    {
                        var prefijo = model.Casillero.Numero.Substring(0, 4);
                        var desde = Convert.ToInt32(model.Casillero.Numero.Substring(5));
                        var hasta = Convert.ToInt32(model.Hasta.Substring(5));
                        for (var i = 0; i <= (hasta - desde); i++)
                        {
                            var numero = prefijo + "-" + (desde + i).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
                            casillerosACrear.Add(new CasilleroDto { Numero = numero, Capacidad = model.Casillero.Capacidad, CentroId = datosUsuario.CentroId });
                    }
                }
                }
                else
                {
                    casillerosACrear.Add(model.Casillero);
                }

                if (ModelState.IsValid)
                {
                    var resultado = servicioComandos.Ejecutar(new CrearCasilleros { Dto = casillerosACrear });
                    if (!resultado.HayErrores)
                    {
                        return new AjaxEditSuccessResult();
                    }
                    ModelState.AgregarErrores(resultado);
                }
            }
            ViewBag.Eliminar = "false";
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerCasillero(id);
            ViewBag.Masivo = "false";
            ViewBag.Eliminar = "false";
            return View(new CasilleroModel { Casillero = aModificar, Masivo = false });
        }

        public ActionResult ModificarMasivo()
        {
            ViewBag.Masivo = "true";
            ViewBag.Eliminar = "false";
            return View("Modificar");
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, CasilleroModel model)
        {
            var casillerosAModificar = new List<CasilleroDto>();
            if (model.Masivo)
            {
                if (model.Casillero.Numero.Substring(0, 4) != model.Hasta.Substring(0, 4))
                {
                    ModelState.AddModelError("Hasta", Textos.Casillero_PrefijosDistintos);
                }
                else
                {
                    var prefijo = model.Casillero.Numero.Substring(0, 4);
                    var desde = Convert.ToInt32(model.Casillero.Numero.Substring(5));
                    var hasta = Convert.ToInt32(model.Hasta.Substring(5));
                    for (var i = 0; i <= (hasta - desde); i++)
                    {
                        var numero = prefijo + "-" + (desde + i).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
                        var casillero = servicio.ObtenerCasilleroPorNumeroYCentro(numero, datosUsuario.CentroId);
                        if (casillero != null)
                        {
                            casillerosAModificar.Add(casillero);
                        }
                }
            }
            }
            else
            {
                casillerosAModificar.Add(model.Casillero);
            }

            if (casillerosAModificar.Count > 0)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCasilleros { Dto = casillerosAModificar, Capacidad = model.Casillero.Capacidad });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            else
            {
                ModelState.AddModelError("Hasta", Textos.Casillero_NingunCasilleroEncontrado);
            }

            ViewBag.Masivo = model.Masivo ? "true" : "false";
            ViewBag.Eliminar = "false";
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCasilleros { Id = new List<int> { id } });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult EliminarMasivo()
        {
            ViewBag.Eliminar = "true";
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult EliminarMasivo(DatosUsuario datosUsuario, CasilleroModel model)
        {
            if (model.Casillero.Numero.Substring(0, 4) != model.Hasta.Substring(0, 4))
            {
                ModelState.AddModelError("Hasta", Textos.Casillero_PrefijosDistintos);
            }
            else
            {
                var casillerosAEliminar = new List<int>();
                var prefijo = model.Casillero.Numero.Substring(0, 4);
                var desde = Convert.ToInt32(model.Casillero.Numero.Substring(5));
                var hasta = Convert.ToInt32(model.Hasta.Substring(5));
                for (var i = 0; i <= (hasta - desde); i++)
                {
                    var numero = prefijo + "-" + (desde + i).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
                    var casillero = servicio.ObtenerCasilleroPorNumeroYCentro(numero, datosUsuario.CentroId);
                    if (casillero != null)
                    {
                        casillerosAEliminar.Add(casillero.Id);
                    }
                }

                if (casillerosAEliminar.Count > 0)
                {
                    var resultado = servicioComandos.Ejecutar(new EliminarCasilleros { Id = casillerosAEliminar });
                    if (!resultado.HayErrores)
                    {
                        return new AjaxEditSuccessResult();
                    }
                    ModelState.AgregarErrores(resultado);
                }
                else
                {
                    ModelState.AddModelError("Hasta", Textos.Casillero_NingunCasilleroEncontrado);
                }
            }
            ViewBag.Eliminar = "true";
            return View(model);
    }
}
}
