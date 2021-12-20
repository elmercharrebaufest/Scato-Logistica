using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstiba : ProcesadorModificar<ModificarEstiba>
    {
        public ProcesadorModificarEstiba(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstiba comando)
        {
            var estiba = Repositorio.Obtener<Estiba>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, estiba);
        }

        protected override void Validar(ModificarEstiba comando, Resultado resultado)
        {

        }
    }
}