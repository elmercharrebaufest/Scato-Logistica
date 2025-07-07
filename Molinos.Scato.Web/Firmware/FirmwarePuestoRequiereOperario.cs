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
    public class FirmwarePuestoRequiereOperario : FirmwareBase
    {
        public FirmwarePuestoRequiereOperario(ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            IRecorridoWorkflow recorridoWorkflow,
            HubClientFactory hubClientFactory) : base(
                log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory, recorridoWorkflow)
        {
        }

        public override void ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            if (lecturaPuestoDeTrabajo.TarjetaValida)
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);
            }

            if (!lecturaPuestoDeTrabajo.TarjetaValida || (lecturaPuestoDeTrabajo.PrimerNumeroDeTarjeta == lecturaPuestoDeTrabajo.NumeroDeTarjeta))
            {
                NotificarLecturaPorSignalR(lecturaPuestoDeTrabajo);
            }
        }
    }
}


