using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmFormatoDeImpresion)]
    public class FormatoDeImpresionController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public FormatoDeImpresionController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoFormatosDeImpresion(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            SetearVista();
            return View(new FormatoDeImpresionDto{Columnas = 1,Filas = 1});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(string formatosDeCampos, FormatoDeImpresionDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                if (formatosDeCampos != "")
                {
                    var listadescuentos = formatosDeCampos.FromJson<FormatoDeCampoDto[]>();
                    tipo.FormatosDeCampo = listadescuentos.Where(x => x._destroy == false).ToList();
                }

                var resultado = servicioComandos.Ejecutar(new CrearFormatoDeImpresion { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista();
            return View(tipo);
        }

        public ActionResult Modificar(int id)
        {
            var tipo = servicio.ObtenerFormatoDeImpresion(id);
            SetearVista();
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(string formatosDeCampos, FormatoDeImpresionDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                if (formatosDeCampos != "")
                {
                    var lista = formatosDeCampos.FromJson<FormatoDeCampoDto[]>();

                    foreach (var descuentoDto in lista.Where(d => d._destroy && d.EsNuevo == false))
                    {
                        servicioComandos.Ejecutar(new EliminarFormatoDeCampo { Id = descuentoDto.Id });
                    }

                    tipo.FormatosDeCampo = lista.Where(x => x._destroy == false).ToList();
                }


                var resultado = servicioComandos.Ejecutar(new ModificarFormatoDeImpresion { Dto = tipo, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.FormatosDeCampos = formatosDeCampos;
            SetearVista();
            return View(tipo);
        }

        public JsonResult ObtenerFormatosDeCampo(int formatoDeImpresionId)
        {
            var formatos = servicio.ListarFormatosDeCampo(formatoDeImpresionId).ToList();
            return Json(formatos, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarFormatoDeImpresion { Id = id, Usuario = datosUsuario.NombreUsuario});
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
        }
        

        private void SetearVista()
        {
            ViewBag.FormatosLetras = servicio.ListarLetras().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.Campos = servicio.ListarCampos().OrderBy(x => x.Descripcion).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.FormatosDePapel = servicio.ListarFormatosDePapel().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.Alineaciones = Enum.GetValues(typeof (Alineacion)).Cast<Alineacion>().Select(x =>
                                                                                               new SelectListItem
                                                                                                   {
                                                                                                       Text = Enum.GetName(typeof (Alineacion),x),
                                                                                                       Value = ((int) x).ToString(CultureInfo.InvariantCulture)
                                                                                                   });

            ViewBag.TiposDeCampo = Enum.GetValues(typeof(TipoDeCampo)).Cast<TipoDeCampo>().Select(x =>
                                                                                               new SelectListItem
                                                                                               {
                                                                                                   Text = Enum.GetName(typeof(TipoDeCampo), x),
                                                                                                   Value = ((int)x).ToString(CultureInfo.InvariantCulture)
                                                                                               });
        }

        public ActionResult Imprimir(int id, int impresora)
        {
            var tipo = servicio.ObtenerFormatoDeImpresion(id);
            var impresoraEnt = servicio.ObtenerImpresora(impresora);
            var resultado = servicioComandos.Ejecutar(new ImprimirDocumentoDeImpresionModelo { Formato = tipo, Impresora = impresoraEnt.Direccion });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
        }

    }
}
