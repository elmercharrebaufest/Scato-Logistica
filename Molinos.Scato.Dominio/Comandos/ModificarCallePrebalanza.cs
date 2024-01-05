using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCallePrebalanza : Comando
    {
        public CalleAutomatismoDto Dto { get; set; }
    }
}