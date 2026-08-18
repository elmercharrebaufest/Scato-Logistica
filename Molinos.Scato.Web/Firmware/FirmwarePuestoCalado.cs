using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePuestoCalado : FirmwareBase
    {
        public FirmwarePuestoCalado(
            ILogger log,
            IServicioRepositorio servicioRepositorio,
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory,
            HubClients hubClients,
            IRecorridoWorkflow recorridoWorkflow)
            : base(log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClients, recorridoWorkflow)
        {
        }

        public override string ProcesarEvento(LecturaPuestoDeTrabajoDto lectura)
        {
            log.Info("Procesando Firmware Puesto Calado. Puesto: {0}, Tarjeta: {1}, Patente: {2}", lectura.PuestoDeTrabajoId, lectura.NumeroDeTarjeta, lectura.Patente);

            if (!lectura.LogIdentificacionVehicularId.HasValue) 
                return Constantes.ResultadoProcesoIdentificacionVehicular.LecturaSinLogId;

            var resultado = comandos.Ejecutar(new EncolarIdentificacionVehicular
            {
                LogIdentificacionVehicularId = lectura.LogIdentificacionVehicularId.Value,
            }) as ResultadoEncolarIdentificacionVehicular;

            if (resultado.HayErrores)
                return resultado.Errores.Values.FirstOrDefault();

            if (resultado.PrimerElementoCola == null)
                return Constantes.ResultadoProcesoIdentificacionVehicular.LecturaSinCola;

            NotificarLecturaCalado(resultado.PrimerElementoCola);

            return Constantes.ResultadoProcesoIdentificacionVehicular.LecturaEncolada;
        }

        private void NotificarLecturaCalado(ColaIdentificacionVehicularDto primerElementoCola)
        {
            hubClientLectura.Invoke("NotificarEncolamientoCalado", primerElementoCola);
        }
    }
}
