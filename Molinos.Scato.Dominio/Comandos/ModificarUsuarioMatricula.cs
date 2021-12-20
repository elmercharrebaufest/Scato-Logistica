using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarUsuarioMatricula : Comando
    {
        public UsuarioMatriculaDto Dto { get; set; }
    }
}
