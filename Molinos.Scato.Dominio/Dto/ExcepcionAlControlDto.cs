using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ExcepcionAlControlDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        public int TransportistaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string RazonSocial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDesc { get; set; }
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        public DateTime FechaDeCarga { get; set; }
        public string Usuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MovStockSap_CentroOrigen")]
        public string CentroNombre { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tipo_Destino")]
        public string TipoDestinoNombre { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Destino")]
        public string Destino { get; set; }

        public int? CentroDestinoId { get; set; }
        public int? ClienteDestinoId { get; set; }
        public string DestinoNombre { get; set; }

        public TipoDestino TipoDestino { get; set; }

        

        public MotivoExcepcionAlControl Motivo { get; set; }
    }
}
