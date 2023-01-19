using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearExcepcionAlControlProveedor : Comando
    {
        public ExcepcionAlControlProveedorDto Dto { get; set; }
    }
}
