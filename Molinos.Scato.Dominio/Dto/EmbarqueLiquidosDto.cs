using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EmbarqueLiquidosDto
    {

        public string NumeroBalanza { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Buque")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Vapor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Commodity")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Bodega")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Bodega { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Exportador")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Exportador { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Fecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }
        public bool EnviadoASap { get; set; }

        // campos para el autocompletar
        public int VaporId { get; set; }
        public int MaterialId { get; set; }
        public int BodegaId { get; set; }
        public int ExportadorId { get; set; }
        public int DestinoId { get; set; }

        public int Peso { get; set; }
    }
}
