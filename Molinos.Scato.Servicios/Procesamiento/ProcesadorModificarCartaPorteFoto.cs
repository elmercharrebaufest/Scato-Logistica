using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCartaPorteFoto : ProcesadorModificar<ModificarCartaPorteFoto>
    {
        public ProcesadorModificarCartaPorteFoto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCartaPorteFoto comando)
        {
            var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.Orden.Id);
            cartaPorte.FotoRutaDestino = comando.Orden.FotoRutaDestino;
        }

        protected override void Validar(ModificarCartaPorteFoto comando, Resultado resultado)
        {
        }
    }
}
