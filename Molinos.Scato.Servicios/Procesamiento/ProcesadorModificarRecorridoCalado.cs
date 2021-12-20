using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoCalado : ProcesadorComando<ModificarRecorridoCalado>
    {
        public ProcesadorModificarRecorridoCalado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoCalado comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId);
                var caladoAAsignar = new Calado
                {
                    WorkflowInstanceId = comando.WorkflowInstanceId,
                    NumeroOrden = ""
                };
                recorridoAEditar.Calado = caladoAAsignar;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al crear el calado vacío para {0}", comando.WorkflowInstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
