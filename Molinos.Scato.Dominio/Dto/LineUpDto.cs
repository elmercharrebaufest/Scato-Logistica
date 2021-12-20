
using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class LineUpDto
    {
        public int Id { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        public PlanoDeCargaDto PlanoDeCarga { get; set; }
        public ModuloDeCargaDto ModuloDeCarga { get; set; }
        public int Ubicacion { get; set; }
        public bool CartaDeSubidaEnviada { get; set; }
        public DateTime? CartaDeSubidaAprobada { get; set; }
        public bool CargaEnSap { get; set; }
        public bool NominacionDePractico { get; set; }
        public bool SeguridadPortuaria { get; set; }
        public bool InspeccionSenasa { get; set; }
        public bool ControlSenasa { get; set; }
        public bool ControlPrivado { get; set; }
        public bool Amarrador { get; set; }
        public bool AgenciaContactada { get; set; }
        public bool PlanoDeCargaEnviado { get; set; }
        public decimal Orden { get; set; }
    }
}
