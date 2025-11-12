using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using NPOI.Util;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoPorContingenciaPay : ProcesadorModificar<ModificarRecorridoPorContingenciaPay>
    {
        public ProcesadorModificarRecorridoPorContingenciaPay(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRecorridoPorContingenciaPay comando)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == comando.InstanceId);
            if (recorrido != null)
            {
                recorrido.IngresoContingenciaPagoMunicipal = true;
                Repositorio.GuardarCambios();
            }
        }

        protected override void Validar(ModificarRecorridoPorContingenciaPay comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Recorrido>(e => e.InstanciaWorkflow == comando.InstanceId))
            {
                resultado.Error("Descripcion", "Recorrido no encontrado");
            }
        }
    }
}
