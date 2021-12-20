using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class NotificacionDto
    {
        public int Id { get; set; }
        public string Mensaje { get; set; }
        public DateTime Hora { get; set; }
        public string HoraVista { get; set; }
        public string HoraServidor { get; set; }
        public bool Leido { get; set; }
        public string Grupo { get; set; }
        public TipoAlerta TipoAlerta { get; set; }
        public int? PuestoId { get; set; }
    }
}
