using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDesactivarContingenciaIngresoPlanta : ProcesadorComando<DesactivarContingenciaIngresoPlanta>
    {
        public ProcesadorDesactivarContingenciaIngresoPlanta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(DesactivarContingenciaIngresoPlanta comando)
        {
            var resultado = new Resultado();
            var actividades = Repositorio.Listar<ActividadPorDispositivo>(x => x.PuestoDeTrabajo.Id == comando.PuestoId);
            var workflows = Repositorio.Listar<MaterialPorWorkflow, int>(x => x.Workflow.Id,  x =>  x.Workflow.Centro.Id == comando.CentroId && x.Material.EsGrano != comando.Granos);
            workflows = workflows.GroupBy(x=>x).Select(x=>x.Key).ToList();
            if (actividades != null)
            {
                foreach (var act in actividades)
                {
                    if (workflows.Contains(act.Workflow.Id))
                    {
                        Repositorio.Remover(act);
                    }
                }
            }
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
