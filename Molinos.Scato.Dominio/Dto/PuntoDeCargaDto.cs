using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class PuntoDeCargaDto
    {
        public  int Id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public  string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Borrado")]
        public  bool? EstadoAutomatismo { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "AutomatismoNoGranos_CantidadMaximaDeCamiones")]
        public  int? CantidadMaximaDeCamiones { get; set; }
        public  bool Borrado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Creacion")]
        public  DateTime FechaCreacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Modificacion")]
        public  DateTime? FechaModificacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Creado_Por")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public  string CreadoPor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Modificado_Por")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string ModificadoPor { get; set; }

        public IList<int> MaterialesId { get; set; }
    }
}