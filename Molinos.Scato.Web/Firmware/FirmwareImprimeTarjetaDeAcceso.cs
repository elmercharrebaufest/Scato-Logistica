using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.ServicioHub;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwareImprimeTarjetaDeAcceso : FirmwareBase
    {
        public FirmwareImprimeTarjetaDeAcceso(ILogger log,
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
            }
            else
            {
                var resultadoImpresion = comandos.Ejecutar(new ImprimirTarjetaDeAcceso
                {
                    Dto = new ImpTarjetaDeAccesoDto
                    {
                        Codigo = "ImpresionTarjetaDeAcceso",
                        Numero = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                        Fecha = DateTime.Now.Formatted(),
                        CentroId = lecturaPuestoDeTrabajo.CentroId,
                        PuestoDeTrabajoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId
                    }
                });
                if (resultadoImpresion.HayErrores)
                {
                    lecturaPuestoDeTrabajo.MensajeError = Textos.ErrorImpresionTarjetaDeAcceso;
                    NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo);
                }
                else
                {
                    comandos.Ejecutar(new CrearCargaDeCupo
                    {
                        Dto = new CargaDeCupoDto
                        {
                            CentroId = lecturaPuestoDeTrabajo.CentroId,
                            PuestoDeTrabajoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
                            Fecha = DateTime.Now,
                            Numero = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                            EstuvoPendiente = true
                        }
                    });
                    EjecutarDispositivos(lecturaPuestoDeTrabajo, new DatosRecorridoDto());
                }
            }
        }
    }
}


