using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwareControlDePesoRechazar : FirmwareControlDePeso
    {
        public FirmwareControlDePesoRechazar(
            ILogger log,
            IServicioRepositorio servicioRepositorio,
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            IServicioActividadFactory<IControlDePesoEsperadoService> factoryControlDePeso,
            IRecorridoWorkflow recorridoWorkflow,
            HubClients hubClients) 
            : base(
                log, 
                servicioRepositorio, 
                workflows, 
                comandos, 
                servicioOrquestador, 
                factory, 
                factoryControlDePeso, 
                recorridoWorkflow, 
                hubClients)
        {
            repesar = true;
        }

    }
}


