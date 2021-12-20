using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEmbarque : Comando
    {
        public EmbarqueDto Dto { get; set; }
    }
}
