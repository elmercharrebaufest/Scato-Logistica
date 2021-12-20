using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HojaDeRutaYerbateraDto : IValidatableObject
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera_NroHojaDeRutaYerbatera")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{4,4}-\d{8,8}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo12Digitos")]
        public string NroHojaDeRutaYerbatera { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public int ProveedorId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Proveedor { get; set; }
        public string ProveedorCodigoSap { get; set; }
        public string ProveedorCuil { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Procedencia")]
        public int ProcedenciaId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Procedencia { get; set; }
        public string ProcedenciaCodigoSap { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Destinatario")]
        public int DestinatarioId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destinatario { get; set; }
        public string DestinatarioCodigoSap { get; set; }
        public string DestinatarioCuil { get; set; }
        public string DestinatarioMail { get; set; }

        public bool DestinatarioEnvioMailEnPesada { get; set; }
        public bool DestinatarioEnvioMailEnAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera_CentroDestino")]
        public int CentroDestinoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CentroDestino { get; set; }
        public string CentroDestinoCodigoSap { get; set; }
        public string CentroDestinoCuit { get; set; }
        public string CentroDestinoDireccion { get; set; }
        public string CentroDestinoLocalidad { get; set; }
        public string CentroDestinoProvincia { get; set; }
        public string CentroDestinoCodigoPostal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista")]
        public int TransportistaId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }
        public string TransportistaCUIT { get; set; }

        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera_Carga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaCarga { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera_Vencimiento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaVencimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "HojaDeRutaYerbatera_Emision")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaEmision { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public string TipoComercial { get; set; }
        public string TipoComercialSentido { get; set; }
        public string TipoComercialCodigoSap { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public string MaterialCodigoSap { get; set; }
        public bool MaterialPideLote { get; set; }
        public bool MaterialPideContrato { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoBruto")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoBrutoOrigen { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PesoTara")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoTaraOrigen { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoNetoOrigen { get; set; }

        public bool EsTransportista { get; set; }
        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        //Se utiliza en CoordinacionController
        public Guid InstanciaWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }
        public int TipoVehiculoInt
        {
            get { return (int)TipoVehiculo; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaCarga > FechaEmision)
            {
                yield return new ValidationResult(string.Format(Textos.Error_FechaMenor + Textos.CartaPorte_FechaIngreso, Textos.CartaPorte_FechaCP), new[] { "FechaCarga" });
            }

            if (FechaEmision.Date > FechaVencimiento.Date)
            {
                yield return new ValidationResult(string.Format(Textos.Error_FechaMayor + FechaEmision.Date + FechaVencimiento.Date, Textos.CartaPorte_FechaVto), new[] { "FechaVencimiento" });
            }
        }
    }
}