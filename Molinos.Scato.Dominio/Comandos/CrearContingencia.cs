using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearContingencia : Comando
    {
        public ContingenciaDto Dto { get; set; }
    }
}
