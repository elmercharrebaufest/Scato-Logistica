using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HidraulicaGraficoDto
    {
        public string Hidraulica { get; set; }
        public int CantidadCamiones { get; set; }
    }
}
