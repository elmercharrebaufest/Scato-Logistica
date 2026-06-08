using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarFinMarcaTiempoPorPuestoDeTrabajo : ProcesadorModificar<ModificarFinMarcaTiempoPorPuestoDeTrabajo>
    {
        public ProcesadorModificarFinMarcaTiempoPorPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log) { }

        protected override void ModificarEntidad(ModificarFinMarcaTiempoPorPuestoDeTrabajo comando)
        {
            MarcaTiempoPorPuestoDeTrabajo registro;
            if (!string.IsNullOrEmpty(comando.NumeroDocumento))
            {
                registro = Repositorio.Obtener<MarcaTiempoPorPuestoDeTrabajo>(x => x.NumeroDocumento == comando.NumeroDocumento && x.FechaFin == null);
            }
            else
            {
                var puesto = ObtenerPuestoDeTrabajo(comando);
                registro = Repositorio.Obtener<MarcaTiempoPorPuestoDeTrabajo>(x => x.PuestoDeTrabajoId == puesto.Id && x.FechaFin == null);
            }
            registro.FechaFin = DateTime.Now;
        }

        protected override void Validar(ModificarFinMarcaTiempoPorPuestoDeTrabajo comando, Resultado resultado)
        {
            if (!string.IsNullOrEmpty(comando.NumeroDocumento))
            {
                if (!Repositorio.Existe<MarcaTiempoPorPuestoDeTrabajo>(x => x.NumeroDocumento == comando.NumeroDocumento && x.FechaFin == null))
                {
                    resultado.Error(nameof(ModificarFinMarcaTiempoPorPuestoDeTrabajo.NumeroDocumento), "No existe una marca de tiempo abierta para el número de documento indicado.");
                }
                return;
            }

            var puesto = ObtenerPuestoDeTrabajo(comando);
            if (puesto == null)
            {
                resultado.Error(nameof(ModificarFinMarcaTiempoPorPuestoDeTrabajo.CodigoDispositivo), "No existe un puesto de trabajo asociado al dispositivo.");
                return;
            }

            if (!Repositorio.Existe<MarcaTiempoPorPuestoDeTrabajo>(x => x.PuestoDeTrabajoId == puesto.Id && x.FechaFin == null))
            {
                resultado.Error(nameof(ModificarFinMarcaTiempoPorPuestoDeTrabajo.CodigoDispositivo), "No existe una marca de tiempo abierta para el puesto de trabajo asociado al dispositivo.");
                return;
            }
        }

        private PuestoDeTrabajo ObtenerPuestoDeTrabajo(ModificarFinMarcaTiempoPorPuestoDeTrabajo comando)
        {
            return Repositorio.Obtener<PuestoDeTrabajo>(p =>
                (
                    p.ConfigSensores != null && 
                    p.ConfigSensores.SensorBarreraEntradaAbajo == comando.CodigoDispositivo
                )
                ||
                (
                    p.VisualizacionBarrera != null && 
                    p.VisualizacionBarrera.SensoresBarreras != null && 
                    p.VisualizacionBarrera.SensoresBarreras.Any(s => s.CodigoDispositivoSensorAbajo == comando.CodigoDispositivo)
                )
                );
        }
    }
}
