using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePuestoDesatendido : FirmwareBase
    {
        public FirmwarePuestoDesatendido(
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
            var resultadoEjecucion = string.Empty;
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                resultadoEjecucion = lecturaPuestoDeTrabajo.MensajeError;
            }
            else
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                resultadoEjecucion = EjecutarPuestoDesatendido(lecturaPuestoDeTrabajo, recorrido);
            }

            NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
            return resultadoEjecucion;
        }
    }
}


