using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialPorWorkflowDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDesc { get; set; }
        
        public string MaterialCodigoSap { get; set; }
        
        public int MaterialId { get; set; }
        
        public bool RequiereAnexoInase { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Workflow")]
        public string WorkflowDesc { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Centro")]
        public string CentroDesc { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CentroId { get; set; }

        public bool RequiereTecnologia { get; set; }
        
        public string Posicion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_EnviaASapAlmacenPredeterminado")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool EnviaASapAlmacenPredeterminado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Cliente")]
        public string ClienteDesc { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ClienteId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsCosecha")]
        public bool EsCosecha { get; set; }
 
        [Display(ResourceType = typeof(Textos), Name = "AnioDesde")]
        public int? VigenciaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AnioHasta")]
        public int? VigenciaHasta { get; set; }
        
        public bool EsDerivadoGranario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ImprimirTicketPesada")]
        public bool ImprimirTicketPesada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ImprimirAsignacionRuta")]
        public bool ImprimirAsignacionRuta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ImprimirMuestraInase")]
        public bool ImprimirMuestraInase { get; set; } 
    }
}