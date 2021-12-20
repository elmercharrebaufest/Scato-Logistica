using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class DescargaUnidadItemDto
    {
        public int? Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_RomaneoNro")]
        public int DescargaUnidadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public string Almacen { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "RomaneoDescargar_LoteNro")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string LoteProveedor { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Bruto")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal? PesoBruto { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Tara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal? PesoTara { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal? PesoNeto { get { return PesoBruto.HasValue && PesoTara.HasValue ? PesoBruto - PesoTara : null; } }
        [Display(ResourceType = typeof(Textos), Name = "Tara")]
        public DateTime Fecha { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_ItemNro")]
        public int ItemNro { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "RomaneoDescargar_FechaFabricacion")]
        public DateTime? FechaFabricacion { get; set; }
        public bool Rechazado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_RemitoNro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string RemitoNro { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RomaneoDescargar_FechaRemito")]
        public DateTime FechaRemito { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TaraRomaneoId { get; set; }

        public string DocMaterial { get; set; }
        public string EjercicioDocMaterial { get; set; }
        public string EjercicioDocMaterialAnulacion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "DescargaUnidad_Unidades")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal Unidades { get; set; }
        public Modalidad ModalidadBalanza { get; set; }
        public string MaterialCodigoSap { get; set; }
    }
}