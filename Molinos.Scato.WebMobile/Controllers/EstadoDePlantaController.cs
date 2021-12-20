using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.UI;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class EstadoDePlantaController : ConsultasController
    {
        private readonly IFirmaProvider firmaProvider;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        private readonly IListaDeWorkflows listaDeWorkflows;

        public EstadoDePlantaController(
            ILogger log,
            IServicioRepositorio servicio,
            IFirmaProvider firmaProvider,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos,
            IListaDeWorkflows listaDeWorkflows

            ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.firmaProvider = firmaProvider;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
            this.listaDeWorkflows = listaDeWorkflows;
        }

        public ActionResult Index()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = Int32.Parse(centro.Value);
            ViewBag.Camaras = servicio.ListarCamarasPorNombrePc("EstadoDePlanta", centroId);
            //ViewBag.Camaras = new List<string>(){ "http://10.10.115.159/axis-cgi/mjpg/video.cgi?resolution=800x600&amp;dummy=1595443238344"
            //    //,
            ////"http://10.10.115.172/axis-cgi/mjpg/video.cgi?resolution=640x480&amp;dummy=1595443238344",
            ////"http://10.10.115.159/axis-cgi/mjpg/video.cgi?resolution=800x600&amp;dummy=1595443238344",
            ////"http://10.10.115.172/axis-cgi/mjpg/video.cgi?resolution=640x480&amp;dummy=1595443238344"
            //};
            return View();
        }

        public ActionResult EstadoDePlanta()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = Int32.Parse(centro.Value);

            log.Debug("Obteniendo Datos gráfico Estado de Cupos, centro: {0}", centroId);

            var materiales = servicio.ListarEstadoPlanta(centroId, true, true);
            var cupos = servicio.ListarEstadoCupos(centroId);
            var cupeados = cupos.Sum(x => x.Otorgados);
            var arribados = cupos.Sum(x => x.ArribadosDia);
            var descargados = cupos.Sum(x => x.Descargados);

            var datosGraficoEstadoCupo = new GraficoEstadoCuposDto
            {
                Cupeados = cupeados,
                Arribados = arribados,
                Descargados = descargados,
                Materiales = materiales
            };
            BuscarPesadaOnline();
            return View(datosGraficoEstadoCupo);
        }

        public JsonResult GenerarGraficoCamionesPorSector()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = Int32.Parse(centro.Value);

            log.Debug("Obteniendo Datos gráfico Camiones en tránsito por Sector, centro: {0}", centroId);


            //var datosGraficoCamionesSector = new GraficoCamionesPorSectorDto
            //{
            //    CantidadEnSector = new List<SectorCantidadCamionesDto>
            //    {
            //        new SectorCantidadCamionesDto{ NombreSector = "Mesa de Entrada", CantidadCamiones = 70},
            //        new SectorCantidadCamionesDto{ NombreSector = "Calado", CantidadCamiones = 65},
            //        new SectorCantidadCamionesDto{ NombreSector = "Playa Externa", CantidadCamiones = 105},
            //        new SectorCantidadCamionesDto{ NombreSector = "En Transito", CantidadCamiones = 12.3},
            //        new SectorCantidadCamionesDto{ NombreSector = "Pesada Bruto", CantidadCamiones = 75},
            //        new SectorCantidadCamionesDto{ NombreSector = "Carga / Descarga", CantidadCamiones = 55}
            //    }
            //};

            //return Json(datosGraficoCamionesSector, JsonRequestBehavior.AllowGet);

            var datosGraficoCamionesSector = new GraficoCamionesPorSectorDto();
            datosGraficoCamionesSector.CantidadEnSector = new List<SectorCantidadCamionesDto>();
            var actividad = listaDeWorkflows.ListarGraficoDePlanta(centroId);
            var actividadesPorSector = actividad.GroupBy(x => x.Sector);
            log.Debug("Calculando datos por sector, centro: {0}", centroId);
            foreach (var sector in actividadesPorSector)
            {
                log.Debug($"Calculando datos por sector {sector.FirstOrDefault().Sector.ToString()}, centro: {centroId}");
                var dto = new SectorCantidadCamionesDto
                {
                    NombreSector = sector.FirstOrDefault().Sector.ToString()
                };
                var sumatoriaDemorados = sector.Select(x => x.CantidadCamionesDemorados).Sum();
                var sumatoriaNoDemorados = sector.Select(x => x.CantidadCamionesNoDemorados).Sum();
                var sumatoriaRangos = sector.Select(x => x.Rango).Sum();
                var cantidadActividadesSector = sector.Count();
                var cantidadCamionesPromedio = (sumatoriaDemorados + sumatoriaNoDemorados) / cantidadActividadesSector;
                var rangoPromedio = sumatoriaRangos / cantidadActividadesSector;
                dto.CantidadCamiones = (100 * cantidadCamionesPromedio) / rangoPromedio;

                datosGraficoCamionesSector.CantidadEnSector.Add(dto);
            }

            return Json(datosGraficoCamionesSector, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerarGraficoCamionesPorHora(GraficoCamionesHoraDto model)
        {
            var centroId = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            log.Debug("Obteniendo Datos Gráfico Camiones Por hora para centro Id : {0}", centroId.Value);
            return Json(servicio.ObtenerGraficoDeCamionesPorHora(model, Int32.Parse(centroId.Value)), JsonRequestBehavior.AllowGet);
        }

        public void BuscarPesadaOnline()
        {
            var balanzas = servicio.ListarBalanzasPuertoReales();
            var datos = new List<ReportePesadaDto>();
            foreach (string balanza in balanzas)
            {
                var dato = servicio.ListarPesadasOnline(balanza);
                datos.Add(dato ?? new ReportePesadaDto { NumeroBalanza = balanza });
            }
            ViewBag.Datos = datos;
        }
    }
}
