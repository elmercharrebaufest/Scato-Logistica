using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ListadoDeCalidades)]
    public class ListadoDeCalidadesController : BaseController
    {
        private readonly ILogger log;

        public ListadoDeCalidadesController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index()
        {
            SetearVista();
            return View();
        }

        [HttpPost]
        public ActionResult Index(ExportacionDeArchivosDto dto)
        {
            ModelState.Remove("IncluirRechazados");
            if (ModelState.IsValid)
            {
                var centrosSeleccionados = dto.Centros.FromJson<ExportacionDeArchivosSelectObjDto[]>() ?? new ExportacionDeArchivosSelectObjDto[0];
                var tiposComercialesSeleccionados = dto.TiposComerciales.FromJson<ExportacionDeArchivosSelectObjDto[]>() ?? new ExportacionDeArchivosSelectObjDto[0];

                if (centrosSeleccionados.Length == 0)
                {
                    ModelState.AddModelError("", Textos.Error_SeleccionarCentro);
                    SetearVista();
                    return View(dto);
                }

                if (dto.MaterialId == 0)
                {
                    ModelState.AddModelError("Material", Textos.Error_SeleccionarMaterial);
                    SetearVista();
                    return View(dto);
                }

                var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    var contenido = GenerarListadoDeCalidades(dto, centrosSeleccionados, tiposComercialesSeleccionados, dto.MaterialId);

                    if (contenido.Length == 0)
                    {
                        ModelState.AddModelError("", Textos.Error_NoExistenDatos);
                        SetearVista();
                        return View(dto);
                    }

                    streamWriter.Write(contenido);
                    streamWriter.Flush();
                    var fileStream = new MemoryStream(memoryStream.ToArray());
                    fileStream.Seek(0, SeekOrigin.Begin);

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    return File(fileStream, "application/octet-stream", Textos.ListadoDeCalidades + ".csv");
                }
            }
            SetearVista();
            return View(dto);
        }

        private StringBuilder GenerarListadoDeCalidades(ExportacionDeArchivosDto dto, IEnumerable<ExportacionDeArchivosSelectObjDto> centrosSeleccionados,
                                                       ExportacionDeArchivosSelectObjDto[] tiposComercialesSeleccionados, int materialId)
        {
            log.Info("Se comienza la generación de archivos . Tipo de archivo: {0}", Textos.ListadoDeCalidades);

            var listadosDeCalidades = servicio.ListarListadoDeCalidades(centrosSeleccionados.Select(x => x.Id).ToList(),
                                                                        dto.FechaDesde,  
                                                                        dto.FechaHasta, 
                                                                        tiposComercialesSeleccionados.Select(x => x.Id).ToList(), materialId);

            return ToCsv(";", listadosDeCalidades);
        }

        private void SetearVista()
        {
            var centros = servicio.ListarCentrosConFiltro(500, 1).ToArray();
            var tiposComerciales = servicio.ListarTiposComercialesConFiltro(500, 1).ToArray();

            ViewBag.Centros = centros.ToJson();
            ViewBag.TiposComerciales = tiposComerciales.ToJson();
        }

        private static StringBuilder ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            var t = typeof(ListadoDeCalidadesDto);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToList();

            var header = "";
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(Dictionary<string, decimal?>) && objectlist.Any())
                {
                    var calidades = ((Dictionary<string, decimal?>)propertyInfo.GetValue(objectlist.First())).OrderBy(x => x.Key);
                    header = calidades.Aggregate(header, (current, calidad) => current + separator + "\"" + calidad.Key + "\"");
                }
                else
                {
                    if (header != "")
                    {
                        header = header + separator;
                    }
                    object[] attrs = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

                    header = header + "\"" + (attrs.Length > 0 ? ((DisplayAttribute)attrs[0]).GetName() : propertyInfo.Name) + "\"";
                }
                
            }

            var csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
            {
                csvdata.AppendLine(ToCsvFields(separator, properties, o));
            }

            return csvdata;
        }

        public static string ToCsvFields(string separator, IEnumerable<PropertyInfo> fields, object o)
        {
            var linie = new StringBuilder();
            var primera = true;
            foreach (var f in fields)
            {
                var x = f.GetValue(o);

                if (f.PropertyType == typeof(Dictionary<string, decimal?>))
                {
                    var calidades = ((Dictionary<string, decimal?>)x).OrderBy(y => y.Key);
                    foreach (var calidad in calidades)
                    {
                        linie.Append(separator);
                        linie.Append(calidad.Value != null ? "\"" + calidad.Value.Value.ToString(CultureInfo.CurrentCulture) + "\"" : "\"" + 0.ToString(CultureInfo.CurrentCulture) + "\"");
                    }
                }
                else
                {
                    if (!primera)
                    {
                        linie.Append(separator);
                    }
                    else
                    {
                        primera = false;
                    }

                    linie.Append(!String.IsNullOrEmpty((string)x) ? "\"" + x + "\"" : "\"\"");                    
                }
            }

            return linie.ToString();
        }

        public JsonResult ObtenerCentros(int pagina)
        {
            var lista = servicio.ListarCentrosConFiltro(30, pagina).ToArray();
            return Json(new { lista }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerTiposComerciales(int pagina)
        {
            var lista = servicio.ListarTiposComercialesConFiltro(30, pagina).ToArray();
            return Json(new { lista }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMaterial(string term)
        {
            var material = servicio.BuscarMaterialTodosLosCentros(term);
            return material != null ? Json(new { label = material.Descripcion, material.Id, material.Descripcion }, JsonRequestBehavior.AllowGet) : Json("", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult BuscarMateriales(string term)
        {
            var materiales = servicio.BuscarMaterialesTodosLosCentros(term);
            return Json(materiales.Select(s => new { label = s.Descripcion, Id = s.Id, s.Descripcion }), JsonRequestBehavior.AllowGet);
        }

    }
}
