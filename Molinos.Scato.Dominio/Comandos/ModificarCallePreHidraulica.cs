using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCallePreHidraulica : Comando
    {
        public CalleAutomatismoDto Dto { get; set; }
    }
}