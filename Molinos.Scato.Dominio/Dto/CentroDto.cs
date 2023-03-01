using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CentroDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_MailBarreras")]
        [StringLength(45, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_MailInvalido")]
        public string MailBarreras { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_Micromuestras")]
        public int? CantEtiquetasMicromuestras { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_Chamico")]
        public int? CodigoDeChamico { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_Insectos")]
        public int? CodigoDeInsectos { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_TenorAzucarino")]
        public int? CodigoDeTenorAzucarino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Sociedad")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Sociedad { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_TiempoMaximo")]
        public int? TiempoMaxEntreActividades { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CasillerosHumedad")]
        public bool CasillerosPorHumedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ConfiguracionDescarga")]
        public bool UsaConfirmacionDescarga { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ReingresaAlPesar")]
        public bool ReingresaPatenteAlPesar { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_EncolaBajaCtgAutomatico")]
        public bool EncolaBajaCtgAutomatico { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ReingresaEnCalado")]
        public bool ReingresaPatenteEnCalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_BinPallet")]
        public bool UsaBinPallet { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_MuestraTerceros")]
        public bool UsaNumeroMuestraTerceros { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ReciboMunicipal")]
        public bool ImprimeReciboMunicipal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ConfirmarCTG")]
        public bool SolicitaConfirmarCTG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_RequiereCupo")]
        public bool RequiereCupo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ValidarCupo")]
        public bool ValidarCupo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ModificaAlmacenEnPesada")]
        public bool ModificaAlmacenEnPesada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_LibroOnccaPorDescripcionCorta")]
        public bool LibroOnccaPorDescripcionCorta { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_NumeroLoteInicial")]
        public int? NumeroLoteInicial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Cuit")]
        public string Cuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Camara")]
        public int CamaraId { get; set; }

        public string CamaraDesc { get; set; }
        public string CamaraCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Provincia")]
        public int? ProvinciaId { get; set; }

        public string ProvinciaDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Localidad")]
        public int? LocalidadId { get; set; }

        public string LocalidadDesc { get; set; }
        public string LocalidadCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Direccion")]
        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Direccion { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_NumeroCAI")]
        public int? NumeroCAI { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        public DateTime? VigenciaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        public DateTime? VigenciaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_VencimientoCAI")]
        [StringLength(45, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_MailInvalido")]
        public string MailVencimientoCAI { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen_CodigoSAP")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CodigoSAPEspecial")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSAPEspecial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_EsVirtual")]
        public bool EsVirtual { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CodigoPostal")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoPostal { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(4, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Centro_NroOrigenCamaraBsAs")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroOrigenCamaraBsAs { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodEstab")]
        public string CodigoEstablecimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_AsignaBalanzaEnComando")]
        public bool AsignaBalanzaEnComando { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_RazonSocial")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string RazonSocial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_NumeroINV")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroINV { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_IngresosBrutos_Corto")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string IngresosBrutos { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CodigoDeAduana")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(3, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoDeAduana { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_DescargaCartaPortePorCtg")]
        public bool DescargaCartaPortePorCtg { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FotoEnMesa")]
        public bool TomarFotoEnMesa { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_HorarioDesde")]
        [Range(0, 24, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Horario")]
        public int? HorarioDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_HorarioHasta")]
        [Range(0, 24, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Horario")]
        public int? HorarioHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CodigoEstacionMeteorologica")]
        public string CodigoEstacionMeteorologica { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "LeerCPDeFoto")]
        public bool LeerCPDeFoto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ToleranciaPatenteLeida")]
        public int? ToleranciaPatenteLeida { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cuit == null)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido), new[] { "Cuit" });
            }
            else
            {
                var cuitInt = Cuit.Replace("-", string.Empty);
                if (cuitInt.Length != 11)
                {
                    yield return new ValidationResult(string.Format(Textos.CuitInvalido), new[] { "Cuit" });
                }
                else
                {
                    int calculado = CalcularDigitoCuit(cuitInt);
                    int digito;
                    if (!int.TryParse(cuitInt.Substring(10), out digito))
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { "Cuit" });
                    }

                    if (calculado != digito)
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { "Cuit" });
                    }
                }
            }
        }

        private int CalcularDigitoCuit(string cuit)
        {
            var mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var nums = cuit.ToCharArray();
            var total = mult.Select((t, i) => int.Parse(nums[i].ToString(CultureInfo.InvariantCulture)) * t).Sum();
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ValidarLimiteDeCreditoVentaEnSAP")]
        public bool ValidarLimiteDeCreditoVentaEnSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ValidarLimiteMinimoDePeso")]
        public bool ValidarLimiteMinimoDePeso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_LimiteMinimoDePeso")]
        public int? LimiteMinimoDePeso { get; set; }

        //[Display(ResourceType = typeof(Textos), Name = "Centro_ModificaPinchazos")]
        public bool ModificaPinchazosPorCalada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InformarCircular")]
        public bool InformaCircular { get; set; }

        public int? MinutosEsperaCircular { get; set; }
        public int? Sucursal { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? Planta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ContingenciaAfipCpe")]
        public bool ContingenciaAfipCpe { get; set; }

        public int? MinutosInactividadCalado { get; set; }
        public int? LimiteCamionesCalado { get; set; }

        public DateTime? FechaEjecucionCacheoCPE { get; set; }
        public string ErrorCacheoAfipCPE { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Ruta_Imagenes")]        
        [StringLength(200, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string FotosPath { get; set; }
       
        public bool AvanzaCpe { get; set; }
        public int? MinutosEsperaPrecalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Domicilio")]
        public int? DomicilioId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_ValidarAutomaticamenteCTGDG")]
        public bool ValidarAutomaticamenteCTGDG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_PlantaDG")]
        public int? PlantaDG { get; set; }
    }
}