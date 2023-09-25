using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarLlamadoVolcableAutomatismoGrano : ProcesadorModificar<ModificarLlamadoVolcableAutomatismoGrano>
    {
        public ProcesadorModificarLlamadoVolcableAutomatismoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarLlamadoVolcableAutomatismoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.Id);
            automatismo.Activo = comando.EsLLamadoVolcable;
        }

        protected override void Validar(ModificarLlamadoVolcableAutomatismoGrano comando, Resultado resultado)
        {
        }
    }
}