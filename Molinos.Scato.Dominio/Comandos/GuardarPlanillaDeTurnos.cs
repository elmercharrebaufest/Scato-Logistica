using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarPlanillaDeTurnos : Comando
    {
        public int IdModuloDeCarga { get; set; }
        public ModuloDeCargaPlanillaDeTurnosDto Dto { get; set; }
    }
}
