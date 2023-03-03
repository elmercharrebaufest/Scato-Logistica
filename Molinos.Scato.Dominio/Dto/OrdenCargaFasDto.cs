using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenCargaFasDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_Patente")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_OrdenCargaFas")]
        public string NumeroOrden { get; set; } 


        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_Cliente")]
        public string ClienteDesc { get; set; }

        public int ClienteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        public string TransportistaDesc { get; set; }

        public int TransportistaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_PatenteAcoplado")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDesc { get; set; }
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_CuitTransporte")]
        public string CuitTransporte { get; set; }

        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public string TipoComercialDesc { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public bool ValidaCompliance { get; set; }
        public int RecorridoId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_KmARecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(4, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string KmARecorrer { get; set; }

        public string LocalidadDestinoDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_Localidad")]
        public int LocalidadDestinoId { get; set; }
        public string ClienteDescripcion { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteLocalidad { get; set; }
        public string ClienteProvincia { get; set; }
        public string ClienteCuit { get; set; }
        public string ClienteCodigoSap { get; set; }
        public string TransportistaCuit { get; set; }
        public string TransportistaDomicilio { get; set; }
        public string TransportistaLocalidad { get; set; }
        public string MaterialUnidadMedida { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }
        public bool VehiculoDemorado { get; set; }
        public string MotivoDemora { get; set; }
        public bool ActualizarTipoVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_DerivadoGranarioHabilitado")]
        public bool DerivadoGranarioHabilitado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_PlantaDGDestino")]
        public int? PlantaDGDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_OrdenDomicilioDestino")]
        public int? OrdenDomicilioDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_CuitPagadorFlete")]
        public string PagadorFlete { get; set; }
        public int? PagadorFleteId { get; set; }
        public bool Inhabilitado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCTG")]
        public string NumeroCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCPE")]
        public string NumeroCPE { get; set; }
        public bool Rechazado { get; set; }
        public string MotivoRechazo { get; set; }
        public string Corredor { get; set; }
        public int? CorredorId { get; set; }
        public string Comisionista { get; set; }
        public int? ComisionistaId { get; set; }
        public string Remitente { get; set; }
        public int? RemitenteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Destinatario_Cuit")]
        public string CuitDestinatario { get; set; }
        public int? TipoDomicilioDestino { get; set; }
        public string TipoYOrdenDestino { get; set; }
    }
}
