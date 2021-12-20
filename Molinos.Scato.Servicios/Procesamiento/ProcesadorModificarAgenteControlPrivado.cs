using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAgenteControlPrivado : ProcesadorModificar<ModificarAgenteControlPrivado>
    {
        public ProcesadorModificarAgenteControlPrivado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAgenteControlPrivado comando)
        {
            var agente = Repositorio.Obtener<AgenteControlPrivado>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, agente);
        }

        protected override void Validar(ModificarAgenteControlPrivado comando, Resultado resultado)
        {

        }
    }
}