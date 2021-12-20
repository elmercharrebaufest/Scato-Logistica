using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionDeTablaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescc { get; set; }
        public int MaterialIdd { get; set; }
        public int Usuario { get; set; }
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ConfiguracionDeTabla_CaracteristicasDeCalidad")]
        public int[] CaracteristicasDeCalidadId { get; set; }
        public string[] CaracteristicasDeCalidadDesc { get; set; }
    }
}
