using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenCargaInternaFasonDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_Numero")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string NumeroOrden { get; set; }

        [FechaAntigua]
        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInternaFason_FechaOrdenCarga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaEmision { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_PatenteAcoplado")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
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

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_Cliente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cliente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ClienteId { get; set; }

        public string ClienteDireccion { get; set; }
        public string ClienteProvincia { get; set; }
        public string ClienteLocalidad { get; set; }
        public string ClienteCodigoSap { get; set; }
        
        public string ClienteCuit { get; set; }
        
        public ChoferDto Chofer { get; set; }

        public int RecorridoId { get; set; }
        //Se utiliza en CoordinacionController
        public Guid InstanciaWorkflow { get; set; }

        public string LocalidadDestinoDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_KmARecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(4, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string KmARecorrer { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AdministracionDistancia_Localidad")]
        public int? LocalidadDestinoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public int TipoVehiculoLectura {  get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_DerivadoGranarioHabilitado")]
        public bool DerivadoGranarioHabilitado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_PlantaDGDestino")]
        public int? PlantaDGDestino { get; set; }
        public int? OrdenDomicilioDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_CuitPagadorFlete")]
        public string PagadorFlete { get; set; }
        public int? PagadorFleteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCTG")]
        public string NumeroCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroCPE")]
        public string NumeroCPE { get; set; }

        public bool Demorado { get; set; }
        public string MotivoDemora { get; set; }
        public bool Rechazado { get; set; }
        public string MotivoRechazo { get; set; }
        public string Corredor { get; set; }
        public int? CorredorId { get; set; }
        public string Comisionista { get; set; }
        public int? ComisionistaId { get; set; }
        public string ComisionistaCodigoSap { get; set; }
        public string Remitente { get; set; }
        public int? RemitenteId { get; set; }
        public string RemitenteCodigoSap { get; set; }

        public int? IntermediarioFleteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_IntermediarioFlete")]
        public string IntermediarioFlete { get; set; }
        
        public int? TipoDomicilioDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_TipoYOrdenDestino")]
        public string TipoYOrdenDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCarga_Destinatario")]
        public string Destinatario { get; set; }
        public int? DestinatarioId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaInterna_NumeroOperaciones")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroOrdenExterno { get; set; }
		public string DestinoMercaderia { get; set; }

       
        public string Observaciones { get; set; }
    }
}