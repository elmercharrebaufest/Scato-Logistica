using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCoordinadorPuerto : ProcesadorModificar<ModificarCoordinadorPuerto>
    {
        public ProcesadorModificarCoordinadorPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCoordinadorPuerto comando)
        {
            var coordinador = Repositorio.Obtener<CoordinadorPuerto>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, coordinador);
        }

        protected override void Validar(ModificarCoordinadorPuerto comando, Resultado resultado)
        {

        }
    }
}