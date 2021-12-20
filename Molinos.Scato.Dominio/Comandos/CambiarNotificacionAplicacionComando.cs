using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class NotificacionAplicacionComando : Comando
    {
        public NotificacionAplicacionDto Dto { get; set; }
        //public int CentroId { get; set; }
    }
}
