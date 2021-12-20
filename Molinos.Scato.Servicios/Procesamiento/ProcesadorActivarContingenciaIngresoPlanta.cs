using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActivarContingenciaIngresoPlanta : ProcesadorComando<ActivarContingenciaIngresoPlanta>
    {
        public ProcesadorActivarContingenciaIngresoPlanta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActivarContingenciaIngresoPlanta comando)
        {
            var resultado = new Resultado();
            var puesto = Repositorio.Obtener<PuestoDeTrabajo>(comando.PuestoId);
            var workflows = Repositorio.Listar<MaterialPorWorkflow,Workflow>(x=>x.Workflow, x=>x.Workflow.Centro.Id == puesto.Centro.Id && x.Material.EsGrano != comando.Granos);
            workflows = workflows.GroupBy(x => x).Select(x => x.Key).ToList();
            foreach (var w in workflows)
            {
                if (!Repositorio.Existe<ActividadPorDispositivo>(x=>x.PuestoDeTrabajo.Id == puesto.Id && x.Workflow.Id == w.Id))
                {
                    var acti = Repositorio.Agregar(new ActividadPorDispositivo
                    {
                        Actividad = "EnTransito",
                        PuestoDeTrabajo = puesto,
                        Workflow = w,
                        Salida = comando.Granos ? "SLOBARINGRESOPTA" : "SLOBARINGRESOPTANOGRANOS"
                    });
                }
            }
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
