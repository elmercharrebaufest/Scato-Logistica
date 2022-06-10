using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Infrastructure.Language;
using Molinos.Scato.Web.EXCEL;
using Molinos.Scato.Dominio.Comandos;
using System.IO;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Controllers
{
    public class ReportesController : BaseController
    {
        private readonly IConfiguracionProvider configuracion;
        public ReportesController(IServicioRepositorio servicio, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.configuracion = configuracion;
        }

        //
        // GET: /Reportes/

        [DatosUsuario]
        [AutorizacionReportes]
        public ActionResult Index(DatosUsuario datosUsuario, String nombreReporte)
        {
            var param = new List<KeyValuePair<string, object>>();

            param.Add(new KeyValuePair<string, object>("Language", SessionManager.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, object>("CentroId", datosUsuario.CentroId));

            if (nombreReporte == "Listado de Arribos a Planta")
            {
                param.Add(new KeyValuePair<string, object>("Location", new UriBuilder(HttpContext.Request.Url) { Path = VirtualPathUtility.ToAbsolute("~/"), Query = "" }));
            }
            ViewBag.NombreReporte = nombreReporte;
            ViewBag.ReportParameterList = param;
            return View();
        }

        [DatosUsuario]
        [AutorizacionReportes]
        public ActionResult PlanillaF515(DatosUsuario datosUsuario)
        {
            var centros = servicio.ListarCentros().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            
            var filtros = SetearVista(datosUsuario.CentroId);

            ViewBag.TiposVehiculo = filtros.tiposDeVehiculo;
            ViewBag.TipoComercial = filtros.tipoComercial;
            ViewBag.Balanzas = filtros.balanzas;
            ViewBag.AlmacenOrigen = filtros.almacenOrigen;
            ViewBag.Workflows = filtros.workflows;
            ViewBag.BocasDestino = filtros.bocasDestino;
            ViewBag.Centros = centros;
            ViewBag.TipoPesada = filtros.tipoPesada;
            ViewBag.RecorridosActivos = filtros.recorridosActivos;
            ViewBag.NivelDetalle = filtros.nivelDetalle;
            ViewBag.Materiales = filtros.materiales;

            return View(new PlanillaF515ViewModel
            {
                FechaSalidaDesde = DateTime.Now.AddDays(-1),
                FechaSalidaHasta = DateTime.Now,
                TipoVehiculoInt = -1,
                CentroId = datosUsuario.CentroId,
                TipoPesadaId = -1
            });
        }

        [AutorizacionReportes]
        [DatosUsuario]
        [AjaxOnly]
        [ActionName("PlanillaF515")]
        public ActionResult Listar(DatosUsuario datosUsuario, PlanillaF515ViewModel filtro)
        {
            if (ModelState.IsValid)
            {
                var param = new List<KeyValuePair<string, object>>();

                param.Add(new KeyValuePair<string, object>("Language", SessionManager.CurrentCulture.Name));
                param.Add(new KeyValuePair<string, object>("CentroId", filtro.CentroId));
                param.Add(new KeyValuePair<string, object>("TipoPesada", filtro.TipoPesadaId));
                param.Add(new KeyValuePair<string, object>("TipoComercial", filtro.TipoComercialId));
                param.Add(new KeyValuePair<string, object>("BalanzaBruto", filtro.BalanzaBrutoId));
                param.Add(new KeyValuePair<string, object>("Material", filtro.MaterialId));
                param.Add(new KeyValuePair<string, object>("BalanzaTara", filtro.BalanzaTaraId));
                param.Add(new KeyValuePair<string, object>("AlmacenOrigen", filtro.AlmacenOrigenId));
                param.Add(new KeyValuePair<string, object>("Workflow", filtro.WorkflowId));
                param.Add(new KeyValuePair<string, object>("AlmacenDestino", filtro.AlmacenDestinoId));
                param.Add(new KeyValuePair<string, object>("TipoVehiculo", filtro.TipoVehiculoInt));
                param.Add(new KeyValuePair<string, object>("Usuario", filtro.Usuario));
                param.Add(new KeyValuePair<string, object>("TitularCP", filtro.ProveedorId));
                param.Add(new KeyValuePair<string, object>("Patente", filtro.Patente));
                param.Add(new KeyValuePair<string, object>("Entregador", filtro.EntregadorId));
                param.Add(new KeyValuePair<string, object>("RecorridosActivos", filtro.RecorridosActivos));
                param.Add(new KeyValuePair<string, object>("Corredor", filtro.CorredorId));
                param.Add(new KeyValuePair<string, object>("NivelDeDetalle", filtro.NivelDeDetalleId));
                param.Add(new KeyValuePair<string, object>("BocaDestino", filtro.BocaDestinoId));
                param.Add(new KeyValuePair<string, object>("FechaIngresoDesde", filtro.FechaIngresoDesde.HasValue ? filtro.FechaIngresoDesde.Value.ToString("MM/dd/yyyy HH:mm") : null));
                param.Add(new KeyValuePair<string, object>("FechaSalidaDesde", filtro.FechaSalidaDesde.HasValue ? filtro.FechaSalidaDesde.Value.ToString("MM/dd/yyyy HH:mm") : null));
                param.Add(new KeyValuePair<string, object>("FechaIngresoHasta", filtro.FechaIngresoHasta.HasValue ? filtro.FechaIngresoHasta.Value.ToString("MM/dd/yyyy HH:mm") : null));
                param.Add(new KeyValuePair<string, object>("FechaSalidaHasta", filtro.FechaSalidaHasta.HasValue ? filtro.FechaSalidaHasta.Value.ToString("MM/dd/yyyy HH:mm") : null));

                return View("ReportViewer", new ReportViewModel
                {
                    ReportPath = "Planilla F-515",
                    ReportParameterList = param.ToEnumerable()
                });
            }
            return null;
        }

        public ActionResult ObtenerFiltros(int centroId)
        {
            return Json(SetearVista(centroId), JsonRequestBehavior.AllowGet);
        }

        private dynamic SetearVista(int centroId)
        {
            SelectListItem todos = new SelectListItem() { Value = "0", Text = "Todos" };
            SelectListItem todosNegativo = new SelectListItem() { Value = "-1", Text = "Todos" };

            var tiposDeVehiculo = Enum.GetValues(typeof(TipoVehiculo)).Cast<TipoVehiculo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            //var tiposDeVehiculo1 = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId).OrderBy(x => x.TipoVehiculo.DisplayText()).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.TipoVehiculo.DisplayText());
            var tipoComercial = servicio.ListarTiposComercialesPorCentro(centroId).OrderBy(x => x.Descripcion).ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            var balanzas = servicio.ListarTodasLasBalanzas(centroId).OrderBy(x => x.Nombre).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            var almacenOrigen = servicio.ListarAlmacenesPorCentro(centroId).OrderBy(x => x.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            var workflows = servicio.ListarWorkflowsPorCentro(centroId).OrderBy(x => x.Descripcion).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            var bocasDestino = servicio.ListarBocasDestino().OrderBy(x => x.Localidad).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Localidad);
            var tipoPesada = new List<SelectListItem>() { new SelectListItem { Text = "Todos", Value = "-1" }, new SelectListItem { Text = "Manual", Value = "0" }, new SelectListItem { Text = "Automatico", Value = "1" } };
            var recorridosActivos = new List<SelectListItem>() { new SelectListItem { Text = "Todos", Value = "0" }, new SelectListItem { Text = "No", Value = "1" }, new SelectListItem { Text = "Si", Value = "2" } };
            var nivelDetalle = new List<SelectListItem>() { new SelectListItem { Text = "Detallado", Value = "1" }, new SelectListItem { Text = "Resumido", Value = "2" }, new SelectListItem { Text = "Detallado Sustentable", Value = "3" }, new SelectListItem { Text = "Detallado Con Boca Destino", Value = "4" } };
            var materiales = servicio.ListarMaterialesFiltroF515(centroId).OrderBy(x => x.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => string.IsNullOrEmpty(x.Descripcion) ? (string.IsNullOrEmpty(x.DescripcionCorta) ? "" : x.DescripcionCorta) : x.Descripcion);

            tiposDeVehiculo.Insert(0, todosNegativo);
            tiposDeVehiculo.First().Selected = true;
            tipoComercial.Insert(0, todos);
            balanzas.Insert(0, todos);
            almacenOrigen.Insert(0, todos);
            workflows.Insert(0, todos);
            bocasDestino.Insert(0, todos);
            materiales.Insert(0, todos);

            return new
            {
                tiposDeVehiculo,
                tipoComercial,
                balanzas,
                almacenOrigen,
                workflows,
                tipoPesada,
                bocasDestino,
                recorridosActivos,
                nivelDetalle,
                materiales
            };
        }

        [DatosUsuario]
        public ActionResult GenerarReporteDetalleDeMovimiento(DatosUsuario datoUsuario, DateTime fecha)
        {
            if(fecha == null)
            {
                fecha = DateTime.Now.Date;
            }
            var reporte = servicio.ReporteDetalleDeMovimiento(datoUsuario.CentroId, fecha);
            var resultado = new ResultadoPrevisualizar();
            var generadorExcel = new ExcelDetalleDeMovimientos();

            generadorExcel.GenerarArchivo(resultado, reporte,fecha);
            byte[] file = resultado.Archivo;
            if (!resultado.HayErrores)
            {
              
                try
                {
                    var path = configuracion.AppSettings["ArchivoDetalleDeMovimiento"];
                    if (!Directory.Exists(path))
                    {
                        //log.Debug("Creando diretorio {0}", path);
                        Directory.CreateDirectory(path);
                    }
                    //log.Debug("Escribiendo archivo {0}", path);
                    using (
                        var outputStream =
                            //System.IO.File.OpenWrite(path + "\\" + reporte.NumeroDeArchivo + ".xls")
                            System.IO.File.OpenWrite(path + "\\" + "DetalleDeMovimiento.xls")
                        )
                    {
                        outputStream.Write(file, 0, file.Length);
                        outputStream.Close();
                    }
                }
                catch (Exception e)
                {
                    //log.Error(e, "Error al publicar Archivo de Movimientos {0}, tipo {1}", archivoId,
                    //          archivoDeMovimientosDto.TipoDeWorkflow);
                    //ModelState.AddModelError("Material",
                    //                         String.Format(Textos.Error_PublicarArchivoDeMovimientos,
                    //                                       reporte.NumeroDeArchivo));
                }

            }
            else
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                }
                return File(file, "application/octet-stream", "DetalleDeMovimiento.xls");
            }

            TempData["Alerta"] = Textos.Exito_Generico;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            return View("Index");
        

        }
        
    }
}
