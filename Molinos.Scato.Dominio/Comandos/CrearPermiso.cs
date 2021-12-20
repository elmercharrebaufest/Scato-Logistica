using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearPermiso : Comando
    {
        public PermisoDto Dto { get; set; }
    }
}
