using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroListaDeWorkflowsDto
    {
        public int? CentroId { get; set; }
        public int? TiempoMaxEntreActividades { get; set; }
        public string NombreUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_SoloDemorados")]
        public bool SoloDemorados { get; set; }

        public string Workflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_ProximaAccion")]
        public string ProximaAccion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Patente")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoDocumentoDeIngreso")]
        public TipoDocumentoIngreso? TipoDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_NumeroDocumentoDeIngreso")]
        public string NumeroDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Calidad")]
        public string Calidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public int? TipoComercialId { get; set; }

        public string TipoComercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_NumeroDeTarjeta")]
        public string NumeroDeTarjeta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Material")]
        public string MaterialDesc { get; set; }

        public int? MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Variedad")]
        public int? TipoVariedadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_SoloNoAsignados")]
        public bool SoloNoAsignados { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_SoloSinDescuentos")]
        public bool SoloSinDescuentos { get; set; }

        public List<string> Columnas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoDeSoja")]
        public TipoDeSoja TipoDeSoja { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoEstado")]
        public TipoEstado TipoEstado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_InstanceState")]
        public EstadoWorkflow? Condicion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_CantidadDeResultados")]
        public CantidadDeResultados CantidadDeResultados { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoDeProteina")]
        public TipoDeProteina TipoDeProteina { get; set; }

        public string CodigoSapMolinos { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Entregador")]
        public string Entregador { get; set; }

        public bool MostrarCamionesPendientes { get; set; }
        public string OrdenarPor { get; set; }
        public DirOrden DirOrden { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoVehiculo")]
        public TipoVehiculo? TipoVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TipoMaterial")]
        public TipoMaterial TipoMaterial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Fila")]
        public int? CalleId { get; set; }

        public bool MostrarCamionesPendientesNoGranos { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_ExcluirRechazados")]
        public bool ExcluirRechazados { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_TieneEntregador")]
        public FiltroEntregador TieneEntregador { get; set; }

    }
}