using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAutomatismoGranos : Comando
    {
        public AutomatismoGranoDto Dto { get; set; }
    }
}