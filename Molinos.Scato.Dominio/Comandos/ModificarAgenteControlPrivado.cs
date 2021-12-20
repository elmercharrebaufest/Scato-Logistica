using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAgenteControlPrivado : Comando
    {
        public AgenteControlPrivadoDto Dto { get; set; }
    }
}

