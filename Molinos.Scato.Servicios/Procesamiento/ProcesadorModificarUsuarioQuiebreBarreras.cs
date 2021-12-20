using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarUsuarioQuiebreBarreras : ProcesadorModificar<ModificarUsuarioQuiebreBarreras>
    {
        public ProcesadorModificarUsuarioQuiebreBarreras(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarUsuarioQuiebreBarreras comando)
        {
            var usuario = Repositorio.Obtener<Usuario>(comando.Dto.Id);

            if (usuario != null)
            {
                usuario.AvisoQuiebreApertura = comando.Dto.AvisoQuiebreApertura;
                usuario.AvisoQuiebreCierre = comando.Dto.AvisoQuiebreCierre;
                usuario.ReasignacionDeTarjeta = comando.Dto.ReasignacionDeTarjeta;
                usuario.AvisoAutorizarTiempoEnTransito = comando.Dto.AvisoAutorizarTiempoEnTransito;
                usuario.AvisoAutorizarTiempoEnTransitoConfirmado = comando.Dto.AvisoAutorizarTiempoEnTransitoConfirmado;
                usuario.AvisoAutorizarTiempoEnTransitoRechazado = comando.Dto.AvisoAutorizarTiempoEnTransitoRechazado;
                usuario.AvisoContingencia = comando.Dto.AvisoContingencia;
                usuario.AvisoEntregaHexano = comando.Dto.AvisoEntregaHexano;
                usuario.AvisoLineUp = comando.Dto.AvisoLineUp;
                usuario.AvisoCambioPinchazosPorCalada = comando.Dto.AvisoCambioPinchazosPorCalada;
            }
        }

        protected override void Validar(ModificarUsuarioQuiebreBarreras comando, Resultado resultado)
        {

        }

    }
}
