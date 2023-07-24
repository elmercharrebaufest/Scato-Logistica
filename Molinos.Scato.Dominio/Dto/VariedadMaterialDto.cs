using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VariedadMaterialDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [StringLength(10)]
        [Display(ResourceType = typeof(Textos), Name = "Codigo_Interno")]
        public string Codigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Activo")]
        public bool Activo { get; set; }

        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(Textos), Name = "Fecha_Creacion")]
        public DateTime FechaCreacion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(Textos), Name = "Fecha_Modificacion")]
        public DateTime? FechaModificacion { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(Textos), Name = "Creado_Por")]
        public string CreadoPor { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(Textos), Name = "Modificado_Por")]
        public string ModificadoPor { get; set; }
    }
}
