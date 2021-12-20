using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVapor : ProcesadorModificar<ModificarVapor>
    {
        public ProcesadorModificarVapor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVapor comando)
        {
            var calle = Repositorio.Obtener<Vapor>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, calle);
        }

        protected override void Validar(ModificarVapor comando, Resultado resultado)
        {

        }
    }
}
