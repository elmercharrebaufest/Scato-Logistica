using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarEstablecimientoProveedor : Comando
    {
        public EstablecimientoProveedorDto Dto { get; set; }
    }
}
