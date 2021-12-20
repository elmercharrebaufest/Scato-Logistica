using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAceptarCamion : ProcesadorComando<AceptarCamion>
    {
        public ProcesadorAceptarCamion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AceptarCamion comando)
        {
            var resultado = new Resultado();
            try
            {
                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                recorrido.Rechazado = false;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Erro al marcar NO rechazado en el recorrido {0}", comando.WorkflowId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
