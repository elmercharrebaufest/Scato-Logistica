using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class LlamarCalle : Comando
    {
        public CalleDto Dto { get; set; }
        public bool Llamada { get; set; }
    }
}
