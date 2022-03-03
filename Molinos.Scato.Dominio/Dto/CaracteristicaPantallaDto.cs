using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaracteristicaPantallaDto
    {
        public string Caracteristica { get; set; }

        public int CaracteristicaId { get; set; }

        public bool EsHumedad { get; set; }

        public string Unidad { get; set; }

        public decimal RangoMin { get; set; }

        public decimal RangoMax { get; set; }

        public string Rango { get { return RangoMin.ToString("g0") + " - " + RangoMax.ToString("g0") + " " + Unidad; } }

        public bool CaladoObligatorio { get; set; }

        public bool AnalisisPreliminar { get; set; }

        public bool EnviaAnalisisObligatorio { get; set; }

        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? ValorCalado { get; set; }

        public decimal? ValorAutomatico { get; set; }

        public int NroDeToma { get; set; }

        public decimal? ToleranciaSinAnalisis { get; set; }

        public decimal? ValorEspecialMin { get; set; }

        public decimal? ValorEspecialMax { get; set; }
        public TipoDispositivo Dispositivo { get; set; }
        public object CaracteristicaNombreNirs { get; set; }

        public Modalidad Modalidad { get; set; }

        public bool AnalisisAutomatico { get; set; }
        public decimal? ToleranciaSinMensaje { get; set; }
        public bool TieneValorCaladoPorDefecto { get; set; }
    }
}
