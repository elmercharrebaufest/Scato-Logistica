using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearNotificacion : Comando
    {
        public NotificacionDto Dto { get; set; }
    }
}
