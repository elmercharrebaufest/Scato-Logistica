using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCosecha : ProcesadorModificar<ModificarCosecha>
    {
        public ProcesadorModificarCosecha(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCosecha comando)
        {
            var cosecha = Repositorio.Obtener<Cosecha>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, cosecha);
        }

        protected override void Validar(ModificarCosecha comando, Resultado resultado)
        {
        }
    }
}
