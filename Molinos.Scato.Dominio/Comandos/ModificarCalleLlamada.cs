using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCalleLlamada : Comando
    {
        public CalleDto Dto { get; set; }
    }
}
