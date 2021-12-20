using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirMicromuestra : Comando
    {
        public ImpIdentificacionMicromuestraDto Dto { get; set; }
        public TipoMicromuestra TipoMicromuestra { get; set; }
    }
}
