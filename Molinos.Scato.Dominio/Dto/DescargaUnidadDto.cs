using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class DescargaUnidadDto
    {
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "NroDescarga")]
        public int NroDescarga { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "NroDescarga")]
        public int OrdenDeDescarga { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_NroPedido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string NroPedido { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public string ProveedorDescripcion { get; set; }

        public int PesoAcumuladoBruto { get; set; }
        public int PesoAcumuladoTara { get; set; }
        public int PesoAcumuladoNeto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Observaciones")]
        [StringLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Estado")]
        public EstadoDescargaUnidad Estado { get; set; }

        public bool CerrarVehiculo { get; set; }
        public Guid WorkflowInstanceId { get; set; }

        public IList<DescargaUnidadItemPedidoDto> DescargaUnidadItemPedidos { get; set; }
        public IList<DescargaUnidadItemDto> DescargaUnidadItems { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InformeDeRecepcion_Imprimir")]
        public bool Imprimir { get; set; }
    }
}