using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePuestoDesatendido : FirmwareBase
    {
        public FirmwarePuestoDesatendido(ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            HubClientFactory hubClientFactory) : base(
                log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory)
        {
        }

        public override void ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                log.Info("Fin - La tarjeta: {0} no es valida: {1}",
                         lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                         lecturaPuestoDeTrabajo.MensajeError);
            }
            else
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                EjecutarPuestoDesatendido(lecturaPuestoDeTrabajo, recorrido);
            }
            NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
        }
    }
}


