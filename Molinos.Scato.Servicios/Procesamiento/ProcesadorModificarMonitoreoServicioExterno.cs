using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMonitoreoServicioExterno : ProcesadorModificar<ModificarMonitoreoServicioExterno>
    {
        public ProcesadorModificarMonitoreoServicioExterno(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMonitoreoServicioExterno comando)
        {
            var service = Repositorio.Obtener<MonitoreoServicioExterno>(comando.Id);
            service.UltimoEstado = comando.UltimoEstado;
            service.UltimaVerificacion = comando.UltimaVerificacion;
        }

        protected override void Validar(ModificarMonitoreoServicioExterno comando, Resultado resultado)
        {
            if (!Repositorio.Existe<MonitoreoServicioExterno>(x => x.Id == comando.Id))
            {
                resultado.Error("ExternalService", "ExternalService inexistente");
            }
        }
    }
}
