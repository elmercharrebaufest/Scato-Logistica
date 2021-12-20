using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarBodega : ProcesadorModificar<ModificarBodega>
    {
        public ProcesadorModificarBodega(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarBodega comando)
        {
            var calle = Repositorio.Obtener<Bodega>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, calle);
        }

        protected override void Validar(ModificarBodega comando, Resultado resultado)
        {

        }
    }
}
