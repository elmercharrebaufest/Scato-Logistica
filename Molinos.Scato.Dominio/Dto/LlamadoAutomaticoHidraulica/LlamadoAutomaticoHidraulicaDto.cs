using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LlamadoAutomaticoHidraulicaDto
    {
        public int Id { get; set; }
        public EstadoHidraulica Estado { get; set; }
        public int HidraulicaId { get; set; }
        public string HidraulicaNombre { get; set; }
        public string UltimaPatenteLlamada { get; set; }
        public DateTime? FechaUltimaModificacionEstado { get; set; }
        public bool ActivoAutomatico { get; set; }
        public bool EsEscalable { get; set; }
        public int CentroId { get; set; }
    }
}