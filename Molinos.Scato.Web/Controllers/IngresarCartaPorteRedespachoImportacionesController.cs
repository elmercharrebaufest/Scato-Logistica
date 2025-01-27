using System.Configuration;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarCartaPorteRedespachoImportaciones)]
    public class IngresarCartaPorteRedespachoImportacionesController : IngresarCartaPorteRedespachoController
    {
        private readonly IConfiguracionProvider configuracionProvider;

        public IngresarCartaPorteRedespachoImportacionesController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion, ZSDWS_SCATO servicioSap, IConfiguracionProvider configuracionProvider, IServicioOrquestador servicioOrquestador)
            : base(log, servicio, factory, servicioComandos, workflows, configuracion, servicioSap, servicioOrquestador)
        {
            this.configuracionProvider = configuracionProvider;
        }

        [DatosUsuario]
        public override ActionResult Index(string workflow, DatosUsuario datosUsuario, string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "", int cargaDeCupoId = 0)
        {
            var firmaCodigoSap = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var titular = configuracionProvider.AppSettings["CodigoSapTerminalPuerto"];
            var destino = configuracionProvider.AppSettings["SanLorenzo"];
            return base.Index(workflow, datosUsuario, firmaCodigoSap, titular, destino, firmaCodigoSap, cargaDeCupoId);
        }

        protected override void SetearVista(WorkflowDto workflow, int centroId)
        {
            base.SetearVista(workflow,centroId);
            ViewBag.DeshabilitarTitular = false;
            ViewBag.DeshabilitarDestinatario = true;
            ViewBag.DeshabilitarEntregador = false;
            ViewBag.ControlarTiempoPorCTG = true;
            ViewBag.RequiereNumeroAduana = false;
            ViewBag.HabilitarSiemprePeso = true;
            ViewBag.HabilitarSiempreProcedencia = true;
            ViewBag.HabilitarSiempreCTG = true;
        }

        [DatosUsuario]
        protected override ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresarCartaPorteRedespachoImportaciones,
                    ActividadXaml = "IngresarCartaPorteRedespachoImportaciones",
                    PuestoDeTrabajoId = usuario.PuestoDeTrabajoId,
                    NombreUsuario = usuario.NombreUsuario
                };
        }

        protected override bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            if (orden.TipoDeWorkflow == TipoDeWorkflow.Egreso && !servicio.ProcedenciaYCodigoValido(usuario.CentroId, orden.CodEstab, orden.ProcedenciaId))
            {
                ModelState.AddModelError("", Textos.Error_ProcedenciaInvalida);
                return false;
            }

            var codigoSapPuertoRosario = ConfigurationManager.AppSettings["CodigoSapPuertoRosario"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId).CodigoSap;
            if (codigoSapTitular != codigoSapPuertoRosario 
                && !(codigoSapTitular == Constantes.ValoresPorDefecto.CodigoSapACA 
                    && orden.CodEstab == Constantes.ValoresPorDefecto.EstablecimientoACA))
            {
                ModelState.AddModelError("", Textos.Error_CCPPRedespacho);
                return false;
            }
            
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }

            return true;
        }
    }
}