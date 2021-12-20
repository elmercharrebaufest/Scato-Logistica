
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarGraficoDePlanta : Comando
    {
        public GraficoDePlantaDto Dto { get; set; }
    }
}
