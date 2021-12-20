using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarUsuarioQuiebreBarreras : Comando
    {
        public UsuarioQuiebreBarrerasDto Dto { get; set; }
    }
}
