using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ComandosEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorFinDeWorkflow : ProcesadorComando<FinDeWorkflow>
    {
        public ProcesadorFinDeWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(FinDeWorkflow comando)
        {
            var resultado = new Resultado();
            try
            {
                Repositorio.EjecutarComando(new ArchivarLogActividad(comando.InstanceId));
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", "Error al archivar el log actividad");
                Log.Error(e, "Error al archivar el log actividad de {0}", comando.InstanceId);
            }
            return resultado;
        }
    }
}
