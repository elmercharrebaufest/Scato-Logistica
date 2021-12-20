using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BalanzaPuertoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaPuerto_UltimaValidacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int UltimaValidacion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_CodigoBalanza")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoBalanza { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoDispositivo { get; set; }
        public int CentroId { get; set; }
        public bool Administrativa { get; set; }
        public int OffSetPlc { get; set; }
        public int IntentosValidacion { get; set; }

        public int UltimoIdInsertado { get; set; }
    }
}
