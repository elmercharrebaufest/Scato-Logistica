using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPuntoDeCarga : ProcesadorModificar<ModificarPuntoDeCarga>
    {
        public ProcesadorModificarPuntoDeCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPuntoDeCarga comando)
        {
            var PuntoDeCargaEditado = Repositorio.Obtener<PuntoDeCarga>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, PuntoDeCargaEditado);
        }

        protected override void Validar(ModificarPuntoDeCarga comando, Resultado resultado)
        {
        }
    }
}