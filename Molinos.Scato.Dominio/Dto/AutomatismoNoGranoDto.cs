using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class AutomatismoNoGranoDto : AuditoriaBaseDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlantaNoGranos")]
        public CalleDto CallePlanta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlantaNoGranos")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CallePlantaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlayaInternaPH")]
        public CalleDto CallePlayaInterna { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePlayaInternaPH")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CallePlayaInternaId { get; set; }

        public bool Activo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        public PuntoDeCargaDto PuntoDeCarga { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuntodeCarga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PuntoDeCargaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public AlmacenDto Almacen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenId { get; set; }
        public bool ActivoLlamado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AutomatismoNoGranos_CantidadDeCamiones")]
        public int CantidadCamiones { get; set; }
    }
}