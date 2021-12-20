using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCalle : Comando
    {
        public CalleDto Dto { get; set; }
    }
}
