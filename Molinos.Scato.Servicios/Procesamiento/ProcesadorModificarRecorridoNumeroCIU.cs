using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoNumeroCIU : ProcesadorComando<ModificarRecorridoNumeroCIU>
    {
        public ProcesadorModificarRecorridoNumeroCIU(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(ModificarRecorridoNumeroCIU comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow ==  comando.InstanceId);
                recorridoAEditar.NumeroCiu = comando.Numero;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificar el CIU del recorrido {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
