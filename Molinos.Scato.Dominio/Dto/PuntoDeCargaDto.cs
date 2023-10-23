using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class PuntoDeCargaDto
    {
        public virtual int Id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public virtual string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Borrado")]
        public virtual bool? EstadoAutomatismo { get; set; }

        public virtual int? CantidadMaximaDeCamiones { get; set; }
        public virtual bool Borrado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Creacion")]
        public virtual DateTime FechaCreacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Modificacion")]
        public virtual DateTime? FechaModificacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Creado_Por")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public virtual string CreadoPor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Modificado_Por")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public virtual string ModificadoPor { get; set; }

        public IList<int> MaterialesId { get; set; }
    }
}