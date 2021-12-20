using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarPlanoDeCarga : Comando
    {
        public PlanoDeCargaDto Dto { get; set; }
    }
}
