using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NroCartaPorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NroCartaPorte { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NroCartaPorteOrigen")]
        [RegularExpression(@"00\d{10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo12DigitosCartaPorte")]
        public string NroCartaPorteOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CEE")]
        public string CEE { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_FechaCP")]
        public DateTime FechaCP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_FechaVto")]
        public DateTime FechaVto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_FechaIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaEmision { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public int TipoVehiculoInt
        {
            get { return (int)TipoVehiculo; }
        }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_OrigenVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public OrigenVehiculo OrigenVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CTG")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public string TipoComercial { get; set; }

        public string TipoComercialSentido { get; set; }

        public string TipoComercialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoCategoria")]
        public int TipoCategoriaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoCategoria")]
        public string TipoCategoria { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        public string Material { get; set; }

        public string MaterialCodigoSap { get; set; }

        public bool MaterialPideLote { get; set; }

        public bool MaterialPideContrato { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Procedencia")]
        public string Procedencia { get; set; }

        public string ProcedenciaCodigoSap { get; set; }

        public string ProvinciaDescripcion { get; set; }

        public string ProvinciaCodigoSap { get; set; }

        public int ProvinciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TitularCartaPorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string TitularCartaPorte { get; set; }

        public string TitularCartaPorteCodigoSap { get; set; }

        public string TitularCartaPorteCuil { get; set; }

        public string TitularCartaPorteMail { get; set; }

        public bool TitularCartaPorteEnvioMailEnPesada { get; set; }
        public bool TitularCartaPorteEnvioMailEnAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Rtte_comercial_productor")]
        public string Intermediario { get; set; }

        public string IntermediarioCodigoSap { get; set; }

        public string IntermediarioCuit { get; set; }

        public string IntermediarioMail { get; set; }

        public bool IntermediarioEnvioMailEnPesada { get; set; }
        public bool IntermediarioEnvioMailEnAnalisis { get; set; }

        //###################################
        [Display(ResourceType = typeof(Textos), Name = "Rtte_comercial_venta_primaria")]
        public string RtteComercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_RtteComercialVentaSecundario")]
        public string RtteComercialVentaSecundario { get; set; }

        public string RtteComercialCodigoSap { get; set; }

        public string RtteComercialCuit { get; set; }

        public string RtteComercialMail { get; set; }
        public string RtteProvinciaLegal { get; set; }
        public string RtteLocalidadLegal { get; set; }
        public string RtteDomicilioLegal { get; set; }

        public bool RtteComercialEnvioMailEnPesada { get; set; }
        public bool RtteComercialEnvioMailEnAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Corredor_primario")]
        public string Corredor { get; set; }

        public string CorredorCodigoSap { get; set; }

        public string CorredorCuil { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CorredorVendedor")]
        public string CorredorVendedor { get; set; }

        public string CorredorVendedorCodigoSap { get; set; }

        public string CorredorVendedorCuil { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_IntermediarioFlete")]
        public string IntermediarioFlete { get; set; }

        public string IntermediarioFleteCodigoSap { get; set; }

        public string IntermediarioFleteCuil { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Entregador")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Entregador { get; set; }

        public string EntregadorCuit { get; set; }

        public string EntregadorMail { get; set; }

        public bool EntregadorEnvioMailEnPesada { get; set; }
        public bool EntregadorEnvioMailEnAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_AgenteCompras")]
        public string AgenteCompras { get; set; }

        public string AgenteComprasCodigoSap { get; set; }

        public string AgenteComprasCuil { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Destinatario")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destinatario { get; set; }

        public string DestinatarioCodigoSap { get; set; }

        public string DestinatarioCuil { get; set; }

        public string DestinatarioMail { get; set; }

        public bool DestinatarioEnvioMailEnPesada { get; set; }
        public bool DestinatarioEnvioMailEnAnalisis { get; set; }

        public string DestinatarioProvincia { get; set; }
        public string DestinatarioLocalidad { get; set; }
        public string DestinatarioDomicilio { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista")]
        public string Transportista { get; set; }

        public string TransportistaCUIT { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_KmRecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? KmRecorrer { get; set; }

        public string KmARecorrer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TarifaTonelada")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? TarifaTonelada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TarifaReferencia")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? TarifaReferencia { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Variedad")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Variedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodEstab")]
        [StringLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodEstab { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_FletePagado")]
        public Boolean? FletePagado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_FleteAPagar")]
        public Boolean? FleteAPagar { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Aparceria")]
        public Boolean Aparceria { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Desvio")]
        public Boolean Desvio { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CalidadEspecial")]
        public Boolean TrigoEspecial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_AcuerdoMarco")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string AcuerdoMarco { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Cosecha")]
        //[Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Cosecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Caratula")]
        public int? Caratula { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodigoAnexo")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoAnexo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tecnologia")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Tecnologia { get; set; }

        public int? TecnologiaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Prestador")]
        public string Prestador { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_BocaDestino")]
        public string PrestadorCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_BocaDestino")]
        public string BocaDestino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destino { get; set; }

        public string DestinoCodigoSap { get; set; }
        public int DestinoId { get; set; }
        public string DestinoCuit { get; set; }
        public string DestinoDireccion { get; set; }
        public string DestinoLocalidad { get; set; }
        public string DestinoProvincia { get; set; }
        public string DestinoCodigoPostal { get; set; }

        public int ProcedenciaId { get; set; }

        public int? TransportistaId { get; set; }

        public int TitularCartaPorteId { get; set; }

        public int? IntermediarioId { get; set; }

        public int RtteComercialId { get; set; }
        public int RtteComercialVentaSecundarioId { get; set; }
        public int RtteComercialProductorId { get; set; }

        public int CorredorId { get; set; }

        public int CorredorVendedorId { get; set; }

        public int IntermediarioFleteId { get; set; }

        public int EntregadorId { get; set; }

        public int AgenteComprasId { get; set; }

        public int DestinatarioId { get; set; }

        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        public int BocaDestinoId { get; set; }

        public int PrestadorId { get; set; }

        public bool EsClienteDestinatario { get; set; }

        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        public ICollection<VehiculoDto> Vehiculos { get; set; }

        //Se utiliza en CoordinacionController y CargarCartaPorteRedespachoDesvioController
        public Guid InstanciaWorkflow { get; set; }

        //Se utiliza en la vista cartaporte

        //Se utiliza en CargarCartaPorteRedespachoDesvioController
        public string Workflow { get; set; }

        //Se utiliza en la vista cartaporte

        public string VehiculoJson
        {
            get
            {
                if (Vehiculos == null)
                {
                    Vehiculos = new Collection<VehiculoDto>();
                }
                return Vehiculos.ToJson();
            }
            set
            {
                Vehiculos = value.FromJson<VehiculoDto[]>();
            }
        }

        public string BocaDestinoOncca { get; set; }

        public string DestinoLocalidadCodigoSap { get; set; }

        public int? MaterialCodigoEspecie { get; set; }

        public int? MaterialTipoGrano { get; set; }

        public bool RequiereAnexoInase { get; set; }

        public bool RequiereTecnologia { get; set; }

        public bool RequiereCupo { get; set; }

        public bool ValidarCupo { get; set; }

        public bool EsTransportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Cupo")]
        public string Cupo { get; set; }

        public bool RequiereNumeroAduana { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_NumeroAduana")]
        public string NumeroAduana { get; set; }

        public string BocaDestinoDomicilio { get; set; }
        public string BocaDestinoLocalidad { get; set; }
        public string BocaDestinoProvincia { get; set; }
        public string CuitProveedorBocaDestino { get; set; }
        public string BocaDestinoCodigoPostal { get; set; }
        public string NombreProveedorBocaDestino { get; set; }
        public string FotoRutaDestino { get; set; }
        public string FotoRutaDestinoDetalle { get; set; }

        public bool TomarFotoEnMesa { get; set; }
        public bool VehiculoDemorado { get; set; }
        public string MotivoDemora { get; set; }
        public bool Cpe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!VehiculoDemorado)
            {
                if (FechaCP == null)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_FechaCP), new[] { "FechaCP" });
                }
                if (TipoDeWorkflow == TipoDeWorkflow.Ingreso && string.IsNullOrEmpty(CTG))
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CTG), new[] { "CTG" });
                }
                if (TipoCategoriaId == 0)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_TipoCategoria), new[] { "TipoCategoriaId" });
                }
                if (Procedencia == null)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_Procedencia), new[] { "Procedencia" });
                }
                if (Cosecha == null)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_Cosecha), new[] { "Cosecha" });
                }
                var dias = Double.Parse(ConfigurationManager.AppSettings["DiasAntiguedadFechaCP"]);
                if (FechaCP != null && FechaCP.AddDays(dias) < FechaEmision)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_FechaCPAntigua, Textos.CartaPorte_FechaCP, String.Format(CultureInfo.CurrentCulture, "{0:d}", FechaEmision.AddDays(-dias)), new[] { "FechaCP" }));
                }
            }

            if (TipoDeWorkflow == TipoDeWorkflow.Ingreso && string.IsNullOrEmpty(CodigoAnexo) && RequiereAnexoInase)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_CodigoAnexo), new[] { "CodigoAnexo" });
            }

            //if (TipoDeWorkflow == TipoDeWorkflow.Ingreso && string.IsNullOrEmpty(CTG))
            //{
            //    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CTG), new[] { "CTG" });
            //}

            //if (Intermediario != null && string.IsNullOrEmpty(RtteComercial))
            //{
            //    yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_RtteComercial), new[] { "RtteComercial" });
            //}

            if (FechaCP != null && FechaCP > FechaEmision)
            {
                yield return new ValidationResult(string.Format(Textos.Error_FechaMenor + Textos.CartaPorte_FechaIngreso, Textos.CartaPorte_FechaCP), new[] { "FechaCP" });
            }

            if (RequiereCupo && (Cupo == null || !(new Regex(@"^MOL[0-9]{4}\/[0-9]{8}$").Match(Cupo).Success)))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_Cupo), new[] { "Cupo" });
            }

            if (RequiereNumeroAduana && string.IsNullOrEmpty(NumeroAduana))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_NumeroAduana), new[] { "NumeroAduana" });
            }

            if (!new Regex(@"^[0-9]{2}-[0-9]{2}$").Match(Cosecha).Success)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_Cosecha), new[] { "Cosecha" });
            }
        }

        public string Patente { get; set; }
        public bool LeerCPDeFoto { get; set; }

        public string NroCartaPorteSAP
        {
            get
            {
                if (this.Cpe)
                {
                    return "000" + this.NroCartaPorte.Trim().Substring(Math.Max(0, this.NroCartaPorte.Trim().Length - 9));
                }

                return this.NroCartaPorte;
            }
        }

        public string SecuenciaSap
        {
            get
            {
                return this.NroCartaPorte.Length == 12 ? this.NroCartaPorte.Substring(0, 2) : "0" + this.NroCartaPorte.Substring(0, 1);
            }
        }

        public string CpeSap
        {
            get
            {
                return string.Format("{0}{1}", this.Sucursal.ToString().PadLeft(5, '0'), this.CTG);
            }
        }

        public int? Sucursal { get; set; }
        public long? NroOrden { get; set; }
        public int DestinoProvinciaCodigoAfip { get; set; }
        public int DestinoLocalidadCodigoAfip { get; set; }
        public bool EsRedespachoPrestamo { get; set; }
        public int? DestinoPlantaAfip { get; set; }
        public string RtteComercialVentaSecundarioCuil { get; set; }
        public string CorredorVendedorSecundarioCuil { get; set; }
        public string RtteComercialProductorCuil { get; set; }
        public int CorredorVendedorSecundarioId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Corredor_secundario")]
        public string CorredorVendedorSecundario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_RtteComercialProductor")]
        public string RtteComercialProductor { get; set; }

        public string RtteComercialVentaSecundarioCodigoSap { get; set; }
        public string RtteComercialProductorCodigoSap { get; set; }
        public string CorredorVendedorSecundarioCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Rtte_comercial_venta_secundaria_2")]
        public string RtteComercialVentaSecundario2 { get; set; }

        public int RtteComercialVentaSecundario2Id { get; set; }
        public string RtteComercialVentaSecundario2Cuil { get; set; }
        public string RtteComercialVentaSecundario2CodigoSap { get; set; }
        public string Observacion { get; set; }
        public string EstadoCpe { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Numero_Operativo")]
        public long? NumeroOperativo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Codigo_Ramal")]
        public string CodigoRamal { get; set; }

        public int? CodigoRamalId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Numero_Precinto")]
        public string NumeroPrecinto { get; set; }

        public int? CodigoRamalAfip { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista_Segundo_Tramo")]
        public string TransportistaTramo2 { get; set; }

        public int? TransportistaTramo2Id { get; set; }
        public string TransportistaTramo2CUIT { get; set; }
        public bool EsTransportistaTramo2 { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista_Pagador_Flete")]
        public string PagadorFlete { get; set; }

        public int? PagadorFleteId { get; set; }
        public string PagadorFleteCuil { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Representante_Recibidor")]
        public string RepresentanteRecibidor { get; set; }

        public int? RepresentanteRecibidorId { get; set; }
        public string RepresentanteRecibidorCuil { get; set; }
        public string FotoRutaSustentable { get; set; }

        public string CupoSalida { get; set; }
        public AdicionalesCartaPorteByPassDto CartaPorteByPass { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_CodigoRENSPA")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d\.\d{5}/\d{2}$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Establecimiento_Error_FormatoCodigoRENSPA")]
        public string CodigoRENSPA { get; set; }
        public string TipoVariedadCodigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TarifaTonelada_Salida")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal? TarifaToneladaSalida { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_KmRecorrer_Salida")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? KmRecorrerSalida { get; set; }
    }
}