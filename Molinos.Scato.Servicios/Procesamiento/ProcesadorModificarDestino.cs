using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarDestino : ProcesadorModificar<ModificarDestino>
    {
        public ProcesadorModificarDestino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarDestino comando)
        {
            var calle = Repositorio.Obtener<Destino>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, calle);
        }

        protected override void Validar(ModificarDestino comando, Resultado resultado)
        {

        }
    }
}
