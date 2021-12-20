using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPermiso : Comando
    {
        public PermisoDto Dto { get; set; }
    }
}
