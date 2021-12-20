using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VariedadPorVinedoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Vinedos_Cosecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cosecha { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Vinedos_Variedad")]
        public string Variedad { get; set; }
        [Required(ErrorMessageResourceType = typeof (Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int VariedadId { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Vinedos_Hectareas")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal Hectareas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Vinedos_AvisoCorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal AvisoCorte { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Vinedos_TopeHectarea")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal TopeHectarea { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Vinedos_KgRecibidos")]
        public decimal KgRecibidos { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "Vinedos_KgARecibir")]
        public decimal KgARecibir { get { return Convert.ToDecimal(Hectareas * TopeHectarea) - KgRecibidos; }}

        [Display(ResourceType = typeof(Textos), Name = "Vinedos_PorcRecibidos")]
        public decimal PorcRecibidos { get
        {
            try
            {
                return (KgRecibidos / (KgARecibir + KgRecibidos)) * 100;
            }
            catch (DivideByZeroException)
            {
                return 0;
            }
        }}

        [Display(ResourceType = typeof(Textos), Name = "Vinedo")]
        public string Vinedo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int VinedoId { get; set; }
    }
}
