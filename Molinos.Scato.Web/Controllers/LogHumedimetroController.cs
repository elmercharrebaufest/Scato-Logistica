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
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class LogHumedimetroController : BaseController
    {
        private readonly ILogger log;

        public LogHumedimetroController(ILogger log, IServicioRepositorio servicio) : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index()
        {
            SetearVista();
            return View();
        }

        public void SetearVista()
        {
            
            IList<CentroDto> listaCentroElementoDefault = new List<CentroDto>();
            listaCentroElementoDefault.Add(new CentroDto{Id = 0, Descripcion = "Todos"});
            IList<CentroDto> listaCentros = servicio.ListarCentros().OrderBy(x => x.Descripcion).ToList();

            foreach (var elemlistaCentro in listaCentros)
            {
                listaCentroElementoDefault.Add(elemlistaCentro);
            }
            
            ViewBag.Centros = listaCentroElementoDefault.ToSelectList(c => c.Id.ToString(CultureInfo.InvariantCulture), c => c.Descripcion);
            
            IList<HumedimetroDto> listaHumedimetrosElementoDefault = new List<HumedimetroDto>();
            listaHumedimetrosElementoDefault.Add(new HumedimetroDto{Id = 0, Descripcion = "Todos"});
            IList<HumedimetroDto> listaHumedimetros = servicio.ListarHumedimetros();

            foreach (var elemlistaHumedimetro in listaHumedimetros)
            {
                listaHumedimetrosElementoDefault.Add(elemlistaHumedimetro);
            }

            ViewBag.Humedimetros = listaHumedimetrosElementoDefault.ToSelectList(h => h.Id.ToString(CultureInfo.InvariantCulture), h => h.Descripcion);
            
        }


        [HttpPost]
        public ActionResult Index(LogHumedimetroDto dto)
        {
            if (ModelState.IsValid)
            {

                var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    var contenido = GenerarListadoDeMuestrasDeHumedad(dto);
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
                    return File(fileStream, "application/octet-stream", Textos.LogHumedimetro + ".csv");  
                }
            }
            SetearVista();
            return View(dto);
        }


        public JsonResult ObtenerListaHumedimetrosPorCentro(int centroId)
        {
            IList<HumedimetroDto> listaHumedimetrosElementoDefault = new List<HumedimetroDto>();
            listaHumedimetrosElementoDefault.Add(new HumedimetroDto { Id = 0, Descripcion = "Todos" });
            IList<HumedimetroDto> listaHumedimetros = servicio.ListarHumedimetrosPorCentro(centroId);

            foreach (var elemlistaHumedimetro in listaHumedimetros)
            {
                listaHumedimetrosElementoDefault.Add(elemlistaHumedimetro);
            }
            
            
            IEnumerable<SelectListItem> lista = listaHumedimetrosElementoDefault.ToSelectList(p => p.Id.ToString(CultureInfo.InvariantCulture),p => p.Descripcion);
            
            
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        private StringBuilder GenerarListadoDeMuestrasDeHumedad(LogHumedimetroDto dto)
        {
            log.Info("Se comienza la generación de archivos . Tipo de archivo: {0}", Textos.LogHumedimetro);

            var listadosDeMuestrasDeHumedad = servicio.ListarListadoDeMuestrasDeHumedad(dto.CentroId,dto.HumedimetroId,dto.FechaDesde, dto.FechaHasta);

            return ToCsv(";", listadosDeMuestrasDeHumedad);
        }


        private static StringBuilder ToCsv(string separator, IEnumerable<MuestraDeHumedadDto> muestras)
        {
            var type = typeof(MuestraDeHumedadDto);

            var properties = type.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToList();
            properties.RemoveAll(info => info.Name.Contains("Id"));
            properties.Remove(properties.Find(p => p.Name == "WorkflowInstanceId"));
            var header = "";
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    header = header + separator;
                    header = header + "\"" + "Fecha" + "\"";
                    header = header + separator;
                    header = header + "\"" + "Hora" + "\"";
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

            foreach (var muestra in muestras)
            {
                csvdata.AppendLine(ToCsvFields(separator, properties, muestra));
            }

            return csvdata;
        }


        private static string ToCsvFields(string separator, IEnumerable<PropertyInfo> fields, MuestraDeHumedadDto muestra)
        {
            muestra.Usuario = muestra.Usuario.ToUpper();
            var linie = new StringBuilder();
            var primera = true;
            foreach (var f in fields)
            {
                var x = f.GetValue(muestra);

                if (f.PropertyType == typeof(DateTime))
                {
                    var fechaYhora = ((DateTime)x);
                    var fecha = fechaYhora.ToShortDateString();
                    linie.Append(separator);
                    linie.Append(!String.IsNullOrEmpty(fecha) ? "\"" + fecha + "\"" : "\"\"");

                    var horaAmpm = fechaYhora.ToString("HH:mm:ss",CultureInfo.InvariantCulture);//Remover a.m y p.m
                    var hora = horaAmpm;

                    linie.Append(separator);
                    linie.Append(!String.IsNullOrEmpty(hora) ? "\"" + hora + "\"" : "\"\"");

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

                    linie.Append(!String.IsNullOrEmpty((x ?? "").ToString()) ? "\"" + x + "\"" : "\"\"");
                }
            }

            return linie.ToString();
        }
    }
}
