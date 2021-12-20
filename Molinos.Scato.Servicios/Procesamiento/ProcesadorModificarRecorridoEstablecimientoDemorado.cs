using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoEstablecimientoDemorado : ProcesadorComando<ModificarRecorridoEstablecimientoDemorado>
    {
        public ProcesadorModificarRecorridoEstablecimientoDemorado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoEstablecimientoDemorado comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId);
                recorridoAEditar.EstablecimientoDemorado = !recorridoAEditar.EstablecimientoDemorado;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificad el establecimiento en el recorrido {0}", comando.WorkflowInstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
