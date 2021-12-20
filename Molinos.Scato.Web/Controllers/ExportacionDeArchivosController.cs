using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models.ArchivosTxt;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ExportacionDeArchivos)]
    public class ExportacionDeArchivosController : BaseController
    {
        private readonly ILogger log;

        public ExportacionDeArchivosController(ILogger log, IServicioRepositorio servicio): base(servicio)
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
                var materialesExcluidos = dto.Materiales.FromJson<ExportacionDeArchivosSelectObjDto[]>() ?? new ExportacionDeArchivosSelectObjDto[0];

                var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    var contenido = dto.TipoArchivo == TipoArchivo.ListadoDeCamiones 
                                                  ? GenerarListadoDeCamiones(dto, centrosSeleccionados, tiposComercialesSeleccionados, materialesExcluidos) 
                                                  : GenerarListadoDePesadas(dto, centrosSeleccionados, tiposComercialesSeleccionados, materialesExcluidos);

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
                    
                    return File(fileStream, "application/octet-stream", dto.TipoArchivo + ((dto.TipoArchivo == TipoArchivo.ListadoDeCamiones) ? ".txt" : ".csv"));
                }
            }
            SetearVista();
            return View(dto);
        }

        private StringBuilder GenerarListadoDeCamiones(ExportacionDeArchivosDto dto, IEnumerable<ExportacionDeArchivosSelectObjDto> centrosSeleccionados,
                                                       ExportacionDeArchivosSelectObjDto[] tiposComercialesSeleccionados,
                                                       ExportacionDeArchivosSelectObjDto[] materialesExcluidos)
        {
            var contenido = new StringBuilder();
            log.Info("Se comienza la generación de archivos . Tipo de archivo: {0}", dto.TipoArchivo);

            var cartasDePorte = servicio.ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(centrosSeleccionados.Select(x => x.Id).ToList(), dto.FechaDesde, dto.FechaHasta, tiposComercialesSeleccionados.Select(x => x.Id).ToList(), materialesExcluidos.Select(x => x.Id).ToList(), dto.IncluirRechazados);
            foreach (var cartaPorte in cartasDePorte)
            {
                contenido.AppendLine(
                    TxtHelper.GetTxtDataRow(
                        new ListadoCamiones
                            {
                                NroCartaDePorte = Convert.ToInt64(cartaPorte.NroCartaDePorte),
                                FechaDeCarga = cartaPorte.FechaDeCarga.ToString("dd/MM/yyyy"),
                                LocalidadDeOrigen = cartaPorte.LocalidadDeOrigen,
                                ProvinciaDeOrigen = cartaPorte.ProvinciaDeOrigen,
                                LocalidadDeDestino = cartaPorte.LocalidadDeDestino,
                                ProvinciaDeDestino = cartaPorte.ProvinciaDeDestino,
                                CuitTransportista = Convert.ToInt64((cartaPorte.CuitTransportista ?? "0").Replace("-", "")),
                                NombreTransportista = cartaPorte.NombreTransportista,
                                CuilDelChofer = Convert.ToInt64((cartaPorte.CuilDelChofer ?? "0").Replace("-", "")),
                                NombreChofer = cartaPorte.NombreChofer
                            }, typeof (ListadoCamiones).GetProperties()));
            }
            return contenido;
        }

        private StringBuilder GenerarListadoDePesadas(ExportacionDeArchivosDto dto, IEnumerable<ExportacionDeArchivosSelectObjDto> centrosSeleccionados,
                                                       ExportacionDeArchivosSelectObjDto[] tiposComercialesSeleccionados,
                                                       ExportacionDeArchivosSelectObjDto[] materialesExcluidos)
        {
            log.Info("Se comienza la generación de archivos . Tipo de archivo: {0}", dto.TipoArchivo);
            
            var listadosDePesadas = servicio.ListarListadoDePesadas(centrosSeleccionados.Select(x => x.Id).ToList(),
                                                                   dto.FechaDesde,
                                                                    dto.FechaHasta,
                                                                    tiposComercialesSeleccionados.Select(x => x.Id).ToList(), 
                                                                    materialesExcluidos.Select(x => x.Id).ToList(), dto.IncluirRechazados);

            return ToCsv(";", listadosDePesadas);
        }

        private void SetearVista()
        {
            var centros = servicio.ListarCentrosConFiltro(500, 1).ToArray();
            var tiposComerciales = servicio.ListarTiposComercialesConFiltro(500, 1).ToArray();
            var materiales = servicio.ListarMaterialesConFiltro(500, 1).ToArray();

            ViewBag.Centros = centros.ToJson();
            ViewBag.TiposComerciales = tiposComerciales.ToJson();
            ViewBag.Materiales = materiales.ToJson();
        }

        private static StringBuilder ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            var t = typeof(ListadoDePesadasDto);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).ToList();
            var header = String.Join(separator, properties.Select(f => "\"" + f.Name + "\"").ToArray());

            var csvdata = new StringBuilder();
            csvdata.AppendLine(header);

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
                    
                linie.Append(!String.IsNullOrEmpty((string) x) ? "\"" + x + "\"": "\"\"");
                   
            }

            return linie.ToString();
        }

        public JsonResult ObtenerCentros(int pagina)
        {
            var lista = servicio.ListarCentrosConFiltro(500, pagina).ToArray();
            return Json(new { lista }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerTiposComerciales(int pagina)
        {
            var lista = servicio.ListarTiposComercialesConFiltro(500, pagina).ToArray();
            return Json(new { lista }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerMateriales(int pagina)
        {
            var lista = servicio.ListarMaterialesConFiltro(500, pagina).ToArray();
            return Json(new { lista }, JsonRequestBehavior.AllowGet);
        }
    }
}
