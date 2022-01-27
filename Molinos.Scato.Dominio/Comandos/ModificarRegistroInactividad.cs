using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRegistroInactividad : Comando
    {
        public RegistroInactividadDto Dto { get; set; }
    }
}
