using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCartaPorteRENSPA : ProcesadorModificar<ModificarCartaPorteRENSPA>
    {
        public ProcesadorModificarCartaPorteRENSPA(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCartaPorteRENSPA comando)
        {
            var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.CartaPorteId);
            cartaPorte.CodigoRENSPA = comando.CodigoRENSPA;
        }

        protected override void Validar(ModificarCartaPorteRENSPA comando, Resultado resultado)
        {
        }
    }
}