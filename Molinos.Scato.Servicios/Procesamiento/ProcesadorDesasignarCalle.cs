using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDesasignarCalle : ProcesadorComando<DesasignarCalle>
    {
        private IAdministradorDeCalles administradorDeCalles;

        public ProcesadorDesasignarCalle(IRepositorio repositorio, IConversor conversor, ILogger log,
            IAdministradorDeCalles administradorDeCalles)
            : base(repositorio, conversor, log)
        {
            this.administradorDeCalles = administradorDeCalles;
        }

        public override Resultado Ejecutar(DesasignarCalle comando)
        {
            Log.Debug($"DesasignarCalle Id : {comando.UltimaAsignacionId} -  InstanciaWorkflow : {comando.InstanciaWorkflow}");
            var asignaciones = Repositorio.Listar<CallePorRecorrido>(
                x => x.FechaEgreso == null &&
                x.Id != comando.UltimaAsignacionId &&
                (x.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow || x.CargaDeCupo.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow));
            Log.Debug($"Asignaciones : {asignaciones.Count}");
            Log.Debug($"DesasignarCalle : {(asignaciones.Count > 0 ? asignaciones.FirstOrDefault().Id : 0)}");
            if (asignaciones.Any())
            {
                Log.Debug($"DesasignarCalle 1");
                foreach (var asignacion in asignaciones)
                {
                    Log.Debug($"DesasignarCalle CallePorRecorrido Id : {asignacion?.Id ?? 0}");
                    Log.Debug($"DesasignarCalle TipoDeCalle : {asignacion?.Id ?? 0}");
                    asignacion.Recorrido = asignacion.Recorrido;
                    asignacion.Calle = asignacion.Calle;
                    asignacion.FechaEgreso = DateTime.Now;
                    if (asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.PreCalado ||
                        asignacion.Calle.TipoCalle == Dominio.Enums.TipoCalle.Circular)
                    {
                        Log.Debug($"DesasignarCalle 2");
                        LLamarSiguienteCallePreCalado(asignacion);
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
            Log.Debug($"Puestos Calados : {(puestosCalados != null ? puestosCalados.Count : 0)}");
            if (puestosCalados.Any())
            {
                Log.Debug($"DesasignarCalle 3");
                var calle = administradorDeCalles.ObtenerSiguienteCalle(materialId);
                Log.Debug($"Siguiente calle : {(calle != null ? calle.Nombre : "Calle nula")}");
                if (calle != null)
                {
                    Log.Debug($"DesasignarCalle 4");
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
            }
        }
    }
}