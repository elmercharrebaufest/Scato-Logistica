using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.ApplicationServer.StoreManagement.Query;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class InstanciaWorkflowDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Workflow_InstanceId")]
        public Guid Id { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Material")]
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_MaterialId")]
        public int? MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Transportista")]
        public string Transportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_CentroId")]
        public int? TransportistaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Cuit")]
        public string Cuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Calidad")]
        public string Calidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Proveedor")]
        public string Proveedor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoDocumentoDeIngreso")]
        public TipoDocumentoIngreso? TipoDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_NumeroDocumentoDeIngreso")]
        public string NumeroDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_CentroId")]
        public int? CentroId { get; set; }
        public int CaladoId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Patente")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_ProximaAccion")]
        public string ProximaAccion { get; set; }
        
        public string DatosProximaActividad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Estado")]
        public InstanceStatus Estado { get; set; }

        public bool EstaSuspendido { get { return Estado == InstanceStatus.Suspended; } }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_FechaCalado")]
        public DateTime FechaCalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_FechaUltimaModificacion")]
        public DateTime? FechaUltimaModificacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_CentroId")]
        public string Centro { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_CentroId")]
        public string CentroCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroTarjeta")]
        public string NumeroDeTarjeta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_MaterialId")]
        public string MaterialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoVehiculo")]
        public TipoVehiculo? TipoVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public string TipoComercial { get; set; }
        public int TipoComercialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Humedad")]
        public string Humedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        public string Workflow { get; set; }
        public string Codigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsSojaSustentable")]
        public bool EsSustentable { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CalidadEspecial")]
        public bool EsEspecial { get; set; }
        public bool TieneDescuentos { get; set; }
        public bool FueAsignado { get; set; }
        public Dictionary<string, string> CaracteristicasDeCalidad { get; set; }
        public string ObtenerValorCaracteristica(string clave)
        {
            if (CaracteristicasDeCalidad != null && CaracteristicasDeCalidad.ContainsKey(clave))
            {
                return CaracteristicasDeCalidad[clave];
            }
            return "";
        }
        public bool EstaDemorado { get; set; }
        public bool Rechazado { get; set; }
        
        public string MotivoDeRechazo { get; set; }
        public bool PagaTicketMunicipal { get; set; }
        public InstanceCondition Condicion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ChoferDNI")]
        public string ChoferDNI { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ChoferNombre")]
        public string ChoferNombre { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Procedencia")]
        public string Procedencia { get; set; }
        public bool Modificado { get; set; }
        public bool EsHumedad { get; set; }
        public bool EsGranosVerdes { get; set; }
        public bool EsGranosDañados { get; set; }
        public bool EsCuerposExtranos { get; set; }
        public bool EsSemillaSoja { get; set; }
        public bool EsProteinaBaja { get; set; }
        public bool EsProteinaMedia { get; set; }
        public bool EsProteinaAlta { get; set; }
        public bool TieneInsectosVivos { get; set; }
        public string TieneEntregador { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Entregador")]
        public string Entregador { get; set; }
        public string RechazoAccion { get; set; }
        public bool Terminado { get; set; }
        public int RecorridoId { get; set; }
        public bool CaracteristicasNoCorrenspodenEspecial { get; set; }
        public bool AnalisisObligatorio { get; set; }
        public string PuestoDeTrabajo { get; set; }
        public bool Reingreso { get; set; }
        public string Calle { get; set; }
        public bool VehiculoDemorado { get; set; }
        public bool LlegoEnHorario { get; set; }
        public bool NoGranos { get; set; }
        public bool CPE { get; set; }
        public string CTG { get; set; }
        public string Proteina { get; set; }
        public string AlmacenDestino { get; set; }
        public string DiferenciaPesoNeto { get; set; }
        public string EsGrano { get; set; }
        public string EsRechazado { get; set; }
        public bool SojaEPA { get; set; }
        public bool SojaIMPO { get; set; }
        public string FleteMOA { get; set; }
        public bool SojaEUDR { get; set; }
        public bool SojaEPAyEUDR { get; set; }
    }
}
