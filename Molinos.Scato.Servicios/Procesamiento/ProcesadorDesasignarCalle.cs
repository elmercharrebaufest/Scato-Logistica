using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDesasignarCalle : ProcesadorComando<DesasignarCalle>
    {
        private IAdministradorDeCalles administradorDeCalles;
        private readonly IServicioComandos servicioComandos;

        public ProcesadorDesasignarCalle(IRepositorio repositorio, IConversor conversor, ILogger log,
            IAdministradorDeCalles administradorDeCalles, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.administradorDeCalles = administradorDeCalles;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(DesasignarCalle comando)
        {
            var asignaciones = Repositorio.Listar<CallePorRecorrido>(
                x => x.FechaEgreso == null &&
                x.Id != comando.UltimaAsignacionId &&
                (x.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow || x.CargaDeCupo.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow));
            if (asignaciones.Any())
            {
                foreach (var asignacion in asignaciones)
                {
                    asignacion.Recorrido = asignacion.Recorrido;
                    asignacion.Calle = asignacion.Calle;
                    asignacion.FechaEgreso = DateTime.Now;
                    if (asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.PreCalado ||
                        asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.Circular ||
                        asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.PostCalado)
                    {
                        //LLamarSiguienteCallePreCalado(asignacion);
                        LiberarFilaSiQuedaVacia(asignacion);
                    }
                }
                Repositorio.GuardarCambios();
            }

            return new Resultado();
        }

        private void LLamarSiguienteCallePreCalado(CallePorRecorrido asignacion)
        {
            var materialId = asignacion?.Recorrido != null ? asignacion?.Recorrido?.Material?.Id ?? 0 : asignacion?.CargaDeCupo?.Material?.Id ?? 0;
            var puestosCalados = Repositorio.Listar<Calle>(x => x.TipoCalle == Dominio.Enums.TipoCalle.Calado && x.Material.Id == materialId && x.Automatica);
            if (puestosCalados.Any())
            {
                var calle = administradorDeCalles.ObtenerSiguienteCalle(materialId);
                if (calle != null)
                {
                    calle.Bloqueada = true;
                    calle.FechaLLamada = DateTime.Now;
                }
            }
        }

        private void LiberarFilaSiQuedaVacia(CallePorRecorrido asignacion)
        {
            var camionesEnFila = Repositorio.Contar<CallePorRecorrido>(x => x.FechaEgreso == null && x.Calle.Id == asignacion.Calle.Id);
            if (camionesEnFila == 1)
            {
                asignacion.Calle.Bloqueada = false;
                asignacion.Calle.FechaLLamada = null;

                if(asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.PreCalado || 
                    asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.Circular)
                {
                    var codigo = Repositorio.ObtenerPrimero<MensajeCartelLedCalador>(x => x.Calle.Id == asignacion.Calle.Id).MensajeCartelLed.Codigo;
                    servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed { CalleId = asignacion.Calle.Id, Codigo = codigo });
                    asignacion.Calle.CalleCalado = null;
                }
            }
        }
    }
}