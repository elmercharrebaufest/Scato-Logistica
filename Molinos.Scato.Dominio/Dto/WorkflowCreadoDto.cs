using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class WorkflowCreadoDto
    {

        [Display(ResourceType = typeof(Textos), Name = "Movimiento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDeWorkflow? Movimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDeBin")]
        public string MaterialDescripcion { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Tipo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoStockBines? TipoAjuste { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Productor")]
        public string CentroDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Productor")]
        public string ProveedorDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Productor")]
        public string VinedoPropioDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CantidadBines")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Cantidad { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Numero")]
        public string Numero { get; set; }

        public int StockCentro { get; set; }

        public int StockProveedor { get; set; }

        public int StockVinedoPropio { get; set; }
    }
}
