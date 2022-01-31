using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BalanzaDto
    {
        public int Id { get; set; }
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Nombre { get; set; }
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Balanza_NombreCorto")]
        public string NombreCorto { get; set; }
        public string Color { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_Tipo")]
        public TipoBalanza TipoBalanza { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_CentroEmisor")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CentroEmisor { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_ToleranciaOrigen")]
        public int? ToleranciaOrigen { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_ToleranciaRechazo")]
        public int? ToleranciaRechazo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_ToleranciaxMil")]
        public decimal? ToleranciaxMil { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_ToleranciaIndianapolis")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ToleranciaIndianapolis { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_CodigoCabezal")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoCabezal { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Balanza_TipoAcceso")]
        public TipoAcceso TipoAcceso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_MaximoValorCereo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaximoValorCereo { get; set; }
        public Modalidad Modalidad { get; set; }
        public bool EstaEnCero { get; set; }
        public int CentroId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_PuestoDeTrabajo")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PuestoDeTrabajo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_Modelo")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Modelo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_NroSerie")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NroSerie { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Balanza_Longitud")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoLongitud { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_Latitud")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoLatitud { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_Lot")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoLot { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_CertificadoDeHabilitacion")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CertificadoDeHabilitacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Balanza_VencimientoDeCertificado")]
        public DateTime? VencimientoDeCertificado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public bool Desactivado { get; set; }

    }
}
 