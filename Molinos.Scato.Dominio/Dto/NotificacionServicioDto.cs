using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class NotificacionServicioDto
    {
        public int Id { get; set; }
        public ServicioDto Mensaje { get; set; }
        public string Grupo { get; set; }
    }
}
