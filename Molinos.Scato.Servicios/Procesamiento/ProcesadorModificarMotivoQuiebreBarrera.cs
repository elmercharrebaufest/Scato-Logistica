using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMotivoQuiebreBarrera : ProcesadorModificar<ModificarMotivoQuiebreBarrera>
    {
        public ProcesadorModificarMotivoQuiebreBarrera(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMotivoQuiebreBarrera comando)
        {
            var motivoQuiebre = Repositorio.Obtener<MotivoQuiebreBarrera>(comando.Dto.Id);
            motivoQuiebre.Motivo = comando.Dto.Motivo;
            motivoQuiebre.Transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId);
            motivoQuiebre.Patente = comando.Dto.Patente != null ? comando.Dto.Patente.ToUpper() : "";
        }

        protected override void Validar(ModificarMotivoQuiebreBarrera comando, Resultado resultado)
        {

        }
    }
}
