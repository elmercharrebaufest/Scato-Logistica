using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class NotificacionesDto
    {
        public IList<NotificacionDto> NotificacionesSobre { get; set; }
        public IList<NotificacionDto> AlertasNoLeidas { get; set; }

        public int Cantidad { get; set; }

    }
}
