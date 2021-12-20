using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarATAPuerto : ProcesadorModificar<ModificarATAPuerto>
    {
        public ProcesadorModificarATAPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarATAPuerto comando)
        {
            var ATA = Repositorio.Obtener<ATAPuerto>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, ATA);
        }

        protected override void Validar(ModificarATAPuerto comando, Resultado resultado)
        {

        }
    }
}