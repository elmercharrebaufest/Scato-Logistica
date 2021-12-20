using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarDecisionEntregadorCalado : ProcesadorModificar<ModificarDecisionEntregadorCalado>
    {
        public ProcesadorModificarDecisionEntregadorCalado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarDecisionEntregadorCalado comando)
        {
            var calado = Repositorio.Obtener<Calado>(comando.CaladoId);
            calado.EstaAutorizadoPorEntregador = comando.Decision;
        }

        protected override void Validar(ModificarDecisionEntregadorCalado comando, Resultado resultado)
        {
        }
    }
}
