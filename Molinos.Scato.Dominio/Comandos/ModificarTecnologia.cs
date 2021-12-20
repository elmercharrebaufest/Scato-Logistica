using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarTecnologia : Comando
    {
        public TecnologiaDto Dto { get; set; }
    }
}
