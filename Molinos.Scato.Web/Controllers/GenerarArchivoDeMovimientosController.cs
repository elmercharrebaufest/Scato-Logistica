using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.EXCEL;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.GenerarArchivosDeMovimientos)]
    public class GenerarArchivoDeMovimientosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos serviciosComandos;
        private readonly IFirmaProvider firma;
        private readonly IConfiguracionProvider configuracion;

        public GenerarArchivoDeMovimientosController(ILogger log, IServicioRepositorio servicio, IServicioComandos serviciosComandos, IFirmaProvider firma,IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.serviciosComandos = serviciosComandos;
            this.firma = firma;
            this.configuracion = configuracion;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Descargar(FiltroArchivoDeMovimientosDto filtroDto, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var materiaPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, filtroDto.MaterialId);
                if (materiaPorCentro == null || !materiaPorCentro.MaterialDeTerceros)
                {
                    ModelState.AddModelError("Material", Textos.MaterialRequiereDeTerceros);
                }
                else if (!servicio.ExistenMovimientosDeTercerosPendientes(filtroDto.MaterialId,filtroDto.TipoDeWorkflow.Value , datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Material", Textos.LoteBiotecnologia_NoHayMuestrasPendientes);
                }
                else
                {
                    filtroDto.NombreUsuario = datosUsuario.NombreUsuario;
                    filtroDto.CentroId = datosUsuario.CentroId;
                    filtroDto.CentroDesc = datosUsuario.CentroDescripcion.Replace(" ", "").Substring(0, 3).ToUpper();
                    filtroDto.Fecha = DateTime.Now;

                    log.Debug("Generando Archivo de Movimientos para material {0}, centro {1}, tipo {2}", filtroDto.Material, filtroDto.CentroDesc,filtroDto.TipoDeWorkflow);
                    var resultado = (ResultadoCrear)serviciosComandos.Ejecutar(new CrearArchivoDeMovimientos { Dto = filtroDto });
                    if (!resultado.HayErrores)
                    {
                        log.Debug("Iniciando descarga de Archivo de Movimientos para material {0}, centro {1}", filtroDto.Material, filtroDto.CentroDesc);
                        return DescargarArchivo(resultado.Id, true);
                    }
                }
            }
            return View("Index", filtroDto);

        }
        
        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult ListadoDetalle(FiltroArchivoDeMovimientosDto filtroDto, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var materiaPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, filtroDto.MaterialId);
                if (materiaPorCentro == null || !materiaPorCentro.MaterialDeTerceros)
                {
                    ModelState.AddModelError("Material", Textos.MaterialRequiereDeTerceros);
                }
                else if (!servicio.ExistenMovimientosDeTercerosPendientes(filtroDto.MaterialId, filtroDto.TipoDeWorkflow.Value, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Material", Textos.LoteBiotecnologia_NoHayMuestrasPendientes);
                }
                else
                {
                    var muestrasElegidas = servicio.ListarMovimientoDeTercerosSinArchivo(filtroDto.MaterialId, filtroDto.TipoDeWorkflow.Value, datosUsuario.CentroId) ?? new List<MovimientoDeTercerosListaDto>();


                    var resultado = new ResultadoPrevisualizar();
                    var generadorExcel = new ExcelMovimientosDeTercerosPendientes();
                    generadorExcel.GenerarArchivo(resultado, muestrasElegidas);

                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    if (!resultado.HayErrores)
                    {
                        byte[] file = resultado.Archivo;
                        return File(file, "application/octet-stream", "MovimientosPendientes.xls");
                    }
                }
            }
            return View("Index", filtroDto);

        }

        [DatosUsuario]
        public ActionResult BuscarArchivo(DatosUsuario datosUsuario, BuscarArchivoDeMovimientoDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("BuscarArchivo")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, BuscarArchivoDeMovimientoDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            filtro.CentroId = datosUsuario.CentroId;
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(BuscarArchivoDeMovimientoDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            if (filtro.FechaHasta.HasValue)
            {
                filtro.FechaHasta = filtro.FechaHasta.Value.AddHours(23);
                filtro.FechaHasta = filtro.FechaHasta.Value.AddMinutes(59);
                filtro.FechaHasta = filtro.FechaHasta.Value.AddSeconds(59);
            }
            ViewBag.Items = servicio.ListarPaginadoArchivoDeMovimiento(filtro, paginacion);
        }

        public ActionResult Seleccionar(int id, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ViewBag.Items = servicio.ListarMuestrasPorArchivoDeMovimientos(id, new Paginacion(ordenarPor, dirOrden, pagina, 20));
            ViewBag.NumeroDeLote = servicio.ObtenerArchivoDeMovimientos(id);
            ViewBag.LoteId = id;
            return View();
        }

        [DatosUsuario]
        public ActionResult GenerarArchivos(int loteId, bool publicar = false)
        {
            return DescargarArchivo(loteId, publicar);
        }

        private ActionResult DescargarArchivo(int archivoId, bool publicar)
        {
            var archivoDeMovimientosDto = servicio.ObtenerMovimientosDeTercerosParaArchivo(archivoId);
            log.Debug("Archivo de Movimientos obtenido  {0}, tipo {1}", archivoId,archivoDeMovimientosDto.TipoDeWorkflow);
            var resultado = new ResultadoPrevisualizar();
            if (archivoDeMovimientosDto.TipoDeWorkflow == TipoDeWorkflow.Egreso)
            {
                var generadorExcel = new ExcelMovimientosDeTercerosEgresos();
                
                generadorExcel.GenerarArchivo(resultado,archivoDeMovimientosDto.Movimientos ?? new List<MovimientoDeTercerosDto>().ToList());
            }
            else
            {
                var generadorExcel = new ExcelMovimientosDeTercerosIngresos();
                generadorExcel.GenerarArchivo(resultado, archivoDeMovimientosDto.Movimientos ?? new List<MovimientoDeTercerosDto>().ToList());
            }
            log.Debug("Archivo de Movimientos renderizado  {0}, tipo {1}", archivoId,archivoDeMovimientosDto.TipoDeWorkflow);

            if (!resultado.HayErrores)
            {
                byte[] file = resultado.Archivo;
                
                if (publicar)
                {
                    log.Debug("Publicando Archivo de Movimientos {0}, tipo {1}", archivoId,
                              archivoDeMovimientosDto.TipoDeWorkflow);
                    try
                    {
                        var path =
                            configuracion.AppSettings[
                                archivoDeMovimientosDto.TipoDeWorkflow == TipoDeWorkflow.Egreso
                                    ? "ArchivoDeMovimientosEgresosPath"
                                    : "ArchivoDeMovimientosIngresosPath"];
                        if (!Directory.Exists(path))
                        {
                            log.Debug("Creando diretorio {0}", path);
                            Directory.CreateDirectory(path);
                        }
                        log.Debug("Escribiendo archivo {0}", path);
                        using (
                            var outputStream =
                                System.IO.File.OpenWrite(path + "\\" + archivoDeMovimientosDto.NumeroDeArchivo + ".xls")
                            )
                        {
                            outputStream.Write(file, 0, file.Length);
                            outputStream.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Error al publicar Archivo de Movimientos {0}, tipo {1}", archivoId,
                                  archivoDeMovimientosDto.TipoDeWorkflow);
                        ModelState.AddModelError("Material",
                                                 String.Format(Textos.Error_PublicarArchivoDeMovimientos,
                                                               archivoDeMovimientosDto.NumeroDeArchivo));
                    }

                }
                else
                {
                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("RetornoExportacion", "ok"));
                    }
                    return File(file, "application/octet-stream", archivoDeMovimientosDto.NumeroDeArchivo + ".xls");
                }
            }
            

            TempData["Alerta"] = Textos.Exito_Generico;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            return View("Index");
        }
    }
}
