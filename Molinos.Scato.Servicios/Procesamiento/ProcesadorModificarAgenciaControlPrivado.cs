using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAgenciaControlPrivado : ProcesadorModificar<ModificarAgenciaControlPrivado>
    {
        public ProcesadorModificarAgenciaControlPrivado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAgenciaControlPrivado comando)
        {
            var agencia = Repositorio.Obtener<AgenciaControlPrivado>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, agencia);
        }

        protected override void Validar(ModificarAgenciaControlPrivado comando, Resultado resultado)
        {

        }
    }
}