using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.ServicioHub.Client;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwareImprimeTarjetaDeAcceso : FirmwareBase
    {
        public FirmwareImprimeTarjetaDeAcceso(
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
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                return lecturaPuestoDeTrabajo.MensajeError;
            }

            var impresionTarjeta = new ImpTarjetaDeAccesoDto
            {
                Codigo = "ImpresionTarjetaDeAcceso",
                Numero = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                Fecha = DateTime.Now.Formatted(),
                CentroId = lecturaPuestoDeTrabajo.CentroId,
                PuestoDeTrabajoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
            };
            var resultadoImpresion = comandos.Ejecutar(new ImprimirTarjetaDeAcceso
            {
                Dto = impresionTarjeta,
                OrigenImpresion = "FirmwareImprimeTarjetaDeAcceso",
            });
            if (resultadoImpresion.HayErrores)
            {
                lecturaPuestoDeTrabajo.MensajeError = Textos.ErrorImpresionTarjetaDeAcceso;
                NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                return lecturaPuestoDeTrabajo.MensajeError;
            }

            var cargaDeCupo = new CargaDeCupoDto
            {
                CentroId = lecturaPuestoDeTrabajo.CentroId,
                PuestoDeTrabajoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
                Fecha = DateTime.Now,
                Numero = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                EstuvoPendiente = true,
            };
            comandos.Ejecutar(new CrearCargaDeCupo
            {
                Dto = cargaDeCupo,
            });
            
            return Constantes.ResultadoProcesoIdentificacionVehicular.PuestoConImpresion;
        }
    }
}


