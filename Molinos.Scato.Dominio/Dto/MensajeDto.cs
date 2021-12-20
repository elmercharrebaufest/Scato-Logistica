using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class MensajeDto
    {
        public string GroupName { get; set; }
        public string Mensaje { get; set; }
        public TipoAlerta TipoAlerta { get; set; }
    }
}
