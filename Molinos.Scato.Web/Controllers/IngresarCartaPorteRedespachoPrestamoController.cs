using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
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
    [Autorizacion(PermisosScato.ActividadIngresarCartaPorteRedespachoPrestamo)]
    public class IngresarCartaPorteRedespachoPrestamoController : IngresarCartaPorteRedespachoController
    {

        public IngresarCartaPorteRedespachoPrestamoController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador)
            : base(log, servicio, factory, servicioComandos, workflows, configuracion, servicioSap, servicioOrquestador)
        {
        }

        protected override void SetearVista(WorkflowDto workflow, int centroId)
        {
            base.SetearVista(workflow, centroId);
            ViewBag.DeshabilitarTitular = true;
            ViewBag.DeshabilitarDestinatario = false;
            ViewBag.DeshabilitarEntregador = false;
        }

        protected override bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);

            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }

            return true;
        }

        [DatosUsuario]
        protected override ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarCartaPorteRedespachoPrestamo,
                ActividadXaml = "IngresarCartaPorteRedespachoPrestamo",
                PuestoDeTrabajoId = usuario.PuestoDeTrabajoId,
                NombreUsuario = usuario.NombreUsuario
            };
        }
    }
}