using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ExportacionDeArchivosINV)]
    public class ExportacionDeArchivosINVController : BaseController
    {

        private readonly ILogger log;

        public ExportacionDeArchivosINVController(ILogger log, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
        }

        public ActionResult Index()
        {
            return View(new ExportacionDeArchivosINVDto { FechaDesde = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)), FechaHasta = DateTime.Now });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(ExportacionDeArchivosINVDto dto, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    var contenido = GenerarArchivoINV(dto, datosUsuario.CentroId);

                    if (contenido.Length == 0)
                    {
                        ModelState.AddModelError("", Textos.Error_NoExistenDatos);
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
                    return File(fileStream, "application/octet-stream", "ciu.txt");
                }
            }
            return View(dto);
        }

        private StringBuilder GenerarArchivoINV(ExportacionDeArchivosINVDto dto, int centroId)
        {
            log.Info("Se comienza la generación de archivos inv desde: {0}, hasta: {1} para el centro {2}", dto.FechaDesde,dto.FechaHasta,centroId);

            var listadosDePesadas = servicio.ListarArchivoINV(dto.FechaDesde, dto.FechaHasta, centroId);

            return ToCsv("|", listadosDePesadas);
        }

        private static StringBuilder ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            var t = typeof(ArchivoINVFilaDto);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToList();

            var csvdata = new StringBuilder();
            foreach (var o in objectlist)
            {
                csvdata.AppendLine(ToCsvFields(separator, properties, o));
            }

            return csvdata;
        }

        private static string ToCsvFields(string separator, IEnumerable<PropertyInfo> fields, object o)
        {
            var linie = new StringBuilder();
            var primera = true;
            foreach (var f in fields)
            {
                var x = f.GetValue(o);

                if (!primera)
                {
                    linie.Append(separator);
                }
                else
                {
                    primera = false;
                }

                linie.Append(!String.IsNullOrEmpty((string)x) ? x : "");

            }

            return linie.ToString();
        }

    }
}
