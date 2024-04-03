using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CalleDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Codigo { get; set; }

        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AutomatismoNoGranos_CantidadDeCamiones")]
        public int CantidadDeCamiones { get; set; }

        public TipoCalle TipoCalle { get; set; }

        public string TipoCalleStr
        { get { return TipoCalle.DisplayEnum(); } }

        public bool Bloqueada { get; set; }

        public bool Deshabilitada { get; set; }

        public bool Automatica { get; set; } = false;
        public int MaterialId { get; set; }

        [Display(Name = "Material")]
        public string MaterialDesc { get; set; }

        public DateTime? FechaLLamada { get; set; }
        public TipoCalidad TipoCalidad { get; set; }

        public int? CaracteristicaDeCalidadId { get; set; }

        [Display(Name = "Caracteristicas de Calidad")]
        public string CaracteristicaDeCalidadDesc { get; set; }

        [Display(Name = "Rango Mínimo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? RangoCaracteristicaCalidadMinimo { get; set; }

        [Display(Name = "Rango Máximo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? RangoCaracteristicaCalidadMaximo { get; set; }

        public int CalleCaladoId { get; set; }
        public string CalleCaladoNombre { get; set; }
        public int? Posicion { get; set; }
        public string ColorFondo { get; set; }
        public string ColorTexto { get; set; }
        public bool ActivoAutomatico { get; set; }

        public bool EsNoEditableGrilla { get; set; }

        public int? AutomatismoTipoLlamadoId { get; set; }

        public AutomatismoTipoLlamadoDto AutomatismoTipoLlamado { get; set; }
    }
}