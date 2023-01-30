using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenCargaInternaDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_Numero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string NumeroOrden { get; set; }

        [FechaAntigua]
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_FechaEmision")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaEmision { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_PatenteAcoplado")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }

        public int TransportistaId { get; set; }
        public bool EsTransportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }

        public string TipoComercialDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        public string MaterialDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destino { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int DestinoId { get; set; }

        public ChoferDto Chofer { get; set; }

        public int RecorridoId { get; set; }
        //Se utiliza en CoordinacionController
        public Guid InstanciaWorkflow { get; set; }

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

        public string LocalidadDestinoDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_KmARecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(4, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string KmARecorrer { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_Localidad")]
        public int LocalidadDestinoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Almacen")]
        public int? Almacen_Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public int? Calle_Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_DerivadoGranarioHabilitado")]
        public bool DerivadoGranarioHabilitado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_PlantaDGDestino")]
        public int? PlantaDGDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_OrdenDomicilioDestino")]
        public int? OrdenDomicilioDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_CuitPagadorFlete")]
        public long? CuitPagadorFlete { get; set;}

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCTG")]
        public string NumeroCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCPE")]
        public string NumeroCPE { get; set; }
    }
}