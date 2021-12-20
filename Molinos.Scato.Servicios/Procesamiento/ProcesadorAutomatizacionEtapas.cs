using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAutomatizacionEtapas : ProcesadorComando<AutomatizacionEtapas>
    {
        public ProcesadorAutomatizacionEtapas(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {

        }

        public override Resultado Ejecutar(AutomatizacionEtapas comando)
        {
            var resultado = new Resultado();

            // Obtengo la configuracion del reposito.
            var configuracion = Repositorio.Listar<ConfiguracionAutomatizacionEtapas>(w => !w.Deshabilitada).ToList();

            //Obtengo las intancias disponibles y que cumplan los filtros.
            var instancias = comando.ListaWorflows
                .Where(w => configuracion
                .Any(c => c.CentroId == w.CentroId && c.Actividad == w.ProximaAccion && (w.FechaUltimaModificacion != null && (comando.Fecha - w.FechaUltimaModificacion).Value.Minutes >= c.MinutosEjecucion) )).ToList();

            var instanciasLog = instancias.Select(s => new { Guid = s.Id, FechaUltimaModificacion = s.FechaUltimaModificacion, Minutos = (comando.Fecha - s.FechaUltimaModificacion).Value.Minutes, ProximaEtapa = s.ProximaAccion }).ToList();

            for (int i = 0; i < instancias.Count; i++)
            {
                var WorkflowIntancia = instancias[i];
                switch (WorkflowIntancia.ProximaAccion)
                {
                    case "Coordinacion":
                        var recorrido = Repositorio.Obtener<Recorrido>(w => w.InstanciaWorkflow == WorkflowIntancia.Id);
                        if(!(recorrido is null))
                        {

                        }
                        //var wfdefinition = WorkflowIntancia.Workflow;
                        break;
                    case "AutorizarDescuentosEntregador":

                        break;
                    default:
                        break;
                }
            }

            return resultado;
        }
    }
}
