using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoNumeroCot : ProcesadorComando<ModificarRecorridoNumeroCot>
    {
        public ProcesadorModificarRecorridoNumeroCot(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(ModificarRecorridoNumeroCot comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarRecorridoNumeroCot con RecorridoId = {0}, Numero COT = {1}", comando.InstanceId, comando.NumeroCot);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.NumeroCot = comando.NumeroCot;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarRecorridoNumeroCot con RecorridoId = {0}, Numero COT = {1} ", comando.InstanceId, comando.NumeroCot);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
