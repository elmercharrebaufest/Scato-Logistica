using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class CargaDeCupoDto
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{10,10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo10Digitos")]
        public string Numero { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Cupo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cupo { set; get; }
        public bool SinCupo { get; set; }
        public string RespuestaSap { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "RequierePuestoDeTrabajo")]
        public int PuestoDeTrabajoId { get; set; }
        public string PuestoDeTrabajo { get; set; }
        public bool SinFotoCartaPorte { get; set; }
        public bool ImprimeCartaPorte { get; set; }
        public bool ImprimeTarjetaDeAcceso { get; set; }
        public bool NoAsignaCalleEnGaritaEntrada { get; set; }
        public DateTime Fecha { get; set; }
        public int CentroId { get; set; }
        public int? MaterialId { get; set; }
        public string ProveedorCuit { get; set; }
        public string ProveedorDescripcion { get; set; }
        public string FechaSap { get; set; }
        public string MaterialDescripcion { get; set; }
        public bool EstuvoPendiente { get; set; }
        public bool Especial { get; set; }
        public string Camara { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Cupo_NumeroDeCartaPorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Cupo_NumeroDeCartaPorte")]
        [RegularExpression(@"00\d{10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo12DigitosCartaPorte")]
        public string NumeroCartaPorte { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAfip")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[A-Za-z]{3}\d{3}$|^[A-Za-z]{2}\d{3}[A-Za-z]{2}$", ErrorMessage = "Formato de patente inválido. Use AAA000 o AA000AA.")]
        public string Patente
        {
            get => _patente;
            set => _patente = value?.Trim().ToUpper();
        }

        private string _patente;
        public string ImagenCartaPorte { get; set; }
        public string ImagenCartaPorteSustentable { get; set; }
        public string FotoRutaDestino { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CTG { get; set; }
        public string CodEstab { get; set; }
        public string RtteComercialCodigoSap { get; set; }
        public string TitularCartaPorteCodigoSap { get; set; }
        public string CentroCodigoSap { get; set; }
        public string FotoCamionRutaDestino { get; set; }
        public bool CircuitoNoGranos { get; set; }
        public bool CPE { get; set; }
        public string CUITSolicitante { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }
        public string FotoRutaSustentable { get; set; }
        public bool IngresoAvanceCPEAutomatico { get; set; }
        public string FleteMOA { get; set; }
        public TipoOrdenCargaNoGranos? TipoOrdenCargaNoGranos { get; set; }
        public bool HayVariosMateriales { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
    }
}
