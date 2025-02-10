using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RecorridoDto : IValidatableObject
    {
        public int Id { get; set; }
        public WorkflowDto Workflow { get; set; }
        public Guid InstanciaWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoIngreso { get; set; }

        public string NumeroDocumentoIngresoLegal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public MaterialDto Material { get; set; }

        public CentroDto Centro { get; set; }
        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public AlmacenDto Almacen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        public TransportistaDto Transportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public TipoComercialDto TipoComercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoBruto")]
        public int? PesoBruto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoTara")]
        public int? PesoTara { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoBrutoOrigen")]
        public int? PesoBrutoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoTaraOrigen")]
        public int? PesoTaraOrigen { get; set; }

        public string PesoBrutoUsuario { get; set; }
        public string PesoTaraUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoTaraBodega")]
        public int? PesoTaraBodega { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaTara")]
        public int? BalanzaTaraId { get; set; }

        public int? PesoNetoBodegaEnLitros { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaBruto")]
        public int? BalanzaBrutoId { get; set; }

        public DateTime? PesoBrutoFecha { get; set; }
        public DateTime? PesoTaraFecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoNetoOrigen")]
        public int? PesoNetoOrigen
        { get { return PesoBrutoOrigen.HasValue && PesoTaraOrigen.HasValue ? PesoBrutoOrigen - PesoTaraOrigen : null; } }

        [Display(ResourceType = typeof(Textos), Name = "PesoNetoCentro")]
        public int? PesoNeto
        { get { return PesoBruto.HasValue && PesoTara.HasValue ? PesoBruto - PesoTara : null; } }

        [Display(ResourceType = typeof(Textos), Name = "PesoDiferencia")]
        public int? PesoDiferencia
        { get { return PesoNetoOrigen - PesoNeto; } }

        public DateTime PesoNetoFecha
        { get { return PesoBrutoFecha.HasValue && PesoTaraFecha.HasValue ? (PesoBrutoFecha.Value > PesoTaraFecha.Value ? PesoBrutoFecha.Value : PesoTaraFecha.Value) : DateTime.MinValue; } }

        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        public string Patente { get; set; }

        public string DatosProximaActividad { get; set; }
        public CaladoDto Calado { get; set; }
        public VehiculoDto Vehiculo { get; set; }
        public AnalisisDeCalidadDto AnalisisDeCalidad { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public bool Rechazado { get; set; }
        public string DocumentoInternoSap { get; set; }
        public string NumeroDeDocumentoSap { get; set; }
        public string NumeroCiu { get; set; }
        public int? PesoNetoTransile { get; set; }
        public bool Terminado { get; set; }
        public bool ControlBalanza { get; set; }
        public string TarjetaDeAcceso { get; set; }
        public int? CalleId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public string CalleDesc { get; set; }

        public List<int> HidraulicasId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Hidraulicas")]
        public string HidraulicasDesc { get; set; }

        public bool EnvioMuestraAuditoria { get; set; }
        public TipoDocumentoIngreso? TipoDocumentoIngresoRelacionado { get; set; }
        public string NumeroDocumentoIngresoRelacionado { get; set; }
        public string NumeroCot { get; set; }
        public bool EsSustentable { get; set; }
        public string Estado
        { get { return Terminado ? "Terminado" : Rechazado ? "Rechazado" : "Activo"; } }
        public bool? PagaTicketMunicipal { get; set; }
        public string Usuario { get; set; }

        public EstablecimientoDto Establecimiento { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }

        public bool EsModificable
        { get { return !Terminado && PesoBruto == null && PesoNeto == null && PesoTara == null && (Calado == null || Calado.FechaCreacion == null || VehiculoDemorado); } }

        public bool EsEliminable
        { get { return (Terminado && !Rechazado) || (!Terminado && PesoBruto == null && PesoNeto == null && PesoTara == null && (Calado == null || Calado.FechaCreacion == null)); } }

        public bool TieneFotoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MotivoAutorizacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string MotivoAutorizacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MotivoTerminarInhabilitacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string MotivoTerminarInhabilitacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return new ValidationResult(string.Format(Textos.MasGrandeQue));
        }

        public bool EstablecimientoDemorado { get; set; }
        public bool VehiculoDemorado { get; set; }
        public bool TipoWorkFlowEgreso { get; set; }
        public bool EnvioMuestraInase { get; set; }
        public bool PasoPorContingenciaPesosExc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        public PuntoDeCargaDto PuntoDeCarga { get; set; }
        public string TipoVariedadDescripcion { get; set; }
        public string TipoVariedadCodigo { get; set; }
        public string MotivoDemora { get; set; }
    }
}