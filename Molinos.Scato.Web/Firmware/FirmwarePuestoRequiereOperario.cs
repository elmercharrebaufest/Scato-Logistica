using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePuestoRequiereOperario : FirmwareBase
    {
        public FirmwarePuestoRequiereOperario(
            ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            IRecorridoWorkflow recorridoWorkflow,
            HubClients hubClients) 
            : base(
                log, 
                servicioRepositorio, 
                workflows, 
                comandos, 
                servicioOrquestador, 
                factory, 
                hubClients, 
                recorridoWorkflow)
        {
        }

        public override string ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            if (lecturaPuestoDeTrabajo.TarjetaValida)
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);
            }

            if (!lecturaPuestoDeTrabajo.TarjetaValida || (lecturaPuestoDeTrabajo.PrimerNumeroDeTarjeta == lecturaPuestoDeTrabajo.NumeroDeTarjeta))
                NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);

            return !lecturaPuestoDeTrabajo.TarjetaValida ? lecturaPuestoDeTrabajo.MensajeError : Constantes.ResultadoProcesoIdentificacionVehicular.LecturaEncolada;
        }
    }
}


