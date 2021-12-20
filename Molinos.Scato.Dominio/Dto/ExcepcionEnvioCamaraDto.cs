using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ExcepcionEnvioCamaraDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Caracteristica")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CaracteristicaId { get; set; }
        public string CaracteristicaDesc { get; set; }
        public List<int> CaracteristicasDeCalidadId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public int TipoComercialId { get; set; }
        public string TipoComercialDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public string ProveedorDesc { get; set; }
        public int ProveedorId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Entregador")]
        public string EntregadorDesc { get; set; }
        public int EntregadorId { get; set; }
    }
}