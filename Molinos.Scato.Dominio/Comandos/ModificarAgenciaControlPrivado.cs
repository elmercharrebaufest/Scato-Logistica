using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAgenciaControlPrivado : Comando
    {
        public AgenciaControlPrivadoDto Dto { get; set; }
    }
}