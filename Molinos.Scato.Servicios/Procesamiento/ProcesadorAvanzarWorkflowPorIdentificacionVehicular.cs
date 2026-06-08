using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    /// <summary>
    /// Avanza el workflow del recorrido asociado a una identificación vehicular.
    /// En Etapa 1 (trazabilidad) valida el contexto operativo y retorna el resultado.
    /// La invocación WCF al servicio de workflow (apertura de barrera) se completa en la
    /// historia de extensión XAML del workflow (funcionalidad 3 del plan).
    /// </summary>
    public class ProcesadorAvanzarWorkflowPorIdentificacionVehicular : ProcesadorComando<AvanzarWorkflowPorIdentificacionVehicular>
    {
        public ProcesadorAvanzarWorkflowPorIdentificacionVehicular(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AvanzarWorkflowPorIdentificacionVehicular comando)
        {
            var resultado = new Resultado();

            var recorrido = Repositorio.Obtener<Recorrido>(comando.RecorridoId);
            if (recorrido == null)
            {
                resultado.Error(nameof(comando.RecorridoId), Textos.Error_RecorridoNoEncontrado);
                return resultado;
            }

            var puesto = Repositorio.Obtener<PuestoDeTrabajo>(comando.PuestoId);
            if (puesto == null)
            {
                resultado.Error(nameof(comando.PuestoId), Textos.PuestoDeTrabajo_NoEncontrado);
                return resultado;
            }

            Log.Debug(
                "IdentificacionVehicular — workflow validado. RecorridoId: {0}, PuestoId: {1}, InstanciaWorkflow: {2}",
                comando.RecorridoId, comando.PuestoId, recorrido.InstanciaWorkflow);

            // TODO (funcionalidad 3): invocar servicio WCF de workflow para apertura de barrera.
            return resultado;
        }
    }
}
