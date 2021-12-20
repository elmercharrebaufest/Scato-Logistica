using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class RomaneoDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_RomaneoNro")]
        public int Numero { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_NroPedido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string NroPedido { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_Observaciones")]
        [StringLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Observaciones { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Romaneo_Estado")]
        public EstadoRomaneo Estado { get; set; }
        public IList<RomaneoItemPedidoDto> RomaneoItemsPedidos { get; set; }
        public IList<RomaneoItemDto> RomaneoItems { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }
        public int ProveedorId { get; set; }
        public bool ProveedorDistinto { get; set; }
        public string ProveedorDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "InformeDeRecepcion_Imprimir")]
        public bool Imprimir { get; set; }
    }
}