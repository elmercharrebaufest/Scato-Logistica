using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.MonitorCPECacheada)]
    public class MonitorCPECacheadaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public MonitorCPECacheadaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, MonitorCPECacheadaFiltroDto filtro, string ordenarPor = "CTG", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            var vm = ObtenerMonitorCPECacheadaVM();
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            filtro.CentroId = datosUsuario.CentroId;
            var resultado = servicio.ListarCPEsCacheadas(filtro, paginacion);
            ViewBag.CamionesPendientes = resultado.CamionesPendientes;
            ViewBag.FechaUltimaEjecucion = resultado.FechaUltimaEjecucion;
            ViewBag.ErrorCacheoAfipCPE = resultado.ErrorCacheoAfipCPE;
            ViewBag.Items = resultado.MonitorCPECacheadaListado;
            return View(vm);
        }

        [AjaxOnly]
        [DatosUsuario]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, MonitorCPECacheadaFiltroDto filtro, string ordenarPor = "CTG", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);
            filtro.CentroId = datosUsuario.CentroId;
            var resultado = servicio.ListarCPEsCacheadas(filtro, paginacion);
            ViewBag.CamionesPendientes = resultado.CamionesPendientes;
            ViewBag.FechaUltimaEjecucion = resultado.FechaUltimaEjecucion;
            ViewBag.ErrorCacheoAfipCPE = resultado.ErrorCacheoAfipCPE;
            ViewBag.Items = resultado.MonitorCPECacheadaListado;
            return View("_Listado", filtro);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult ProcesarCacheado(int id, DatosUsuario datosUsuario)
        {
            var cpe = servicio.ObtenerCartaPorteElectronica(id);
            var result = ProcesarCPECacheada(datosUsuario.CentroId, cpe);
            return Json(result);
        }

        [DatosUsuario]
        public ActionResult VerCPE(int id)
        {
            var cpe = servicio.ObtenerCartaPorteElectronica(id);
            var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = cpe.NroCtg }) as ResultadoConsultarImagenCpe;
            var imagenBase64 = Convert.ToBase64String(cartaPorteImagen.PdfImage);
            return View("CPE", model: imagenBase64);
        }

        private bool ProcesarCPECacheada(int centroId, CartaPorteElectronicaDto cpe)
        {
            var intentos = 0;
            var result = false;

            while (!result && intentos < 3)
            {
                try
                {
                    var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital()
                    {
                        CentroId = centroId,
                        TipoVehiculo = cpe.TipoCartaPorte == 79 ? (int)TipoVehiculo.Tren : (int)TipoVehiculo.Camión,
                        NroCtg = cpe.NroCtg,
                        FechaUltimaActualizacion = cpe.FechaUltimaActualizacion ?? DateTime.Now
                    });

                    result = !resultado.HayErrores;
                }
                catch (Exception ex)
                {
                    log.Error("Ocurrio un problema al procesor desde el Monitor CPE la carta {0}: {1}", cpe.NroCtg, ex.Message);
                    result = false;
                }
                finally
                {
                    intentos++;
                }
            }
            return result;
        }

        private MonitorCPECacheadaViewModel ObtenerMonitorCPECacheadaVM()
        {
            var materiales = servicio.ListarMaterialGranosConCodigoONCCA();

            var vm = new MonitorCPECacheadaViewModel()
            {
                Data = new MonitorCPECacheadaFiltroDto(),
                MaterialList = materiales.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion),
            };

            return vm;
        }
    }
}