using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    [DataContract]
    public sealed class PrecintoDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Guid WorkflowInstanceId { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Precinto")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroPrecinto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Precinto_Detalle")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Detalle { get; set; }

        [DataMember(Name = "_destroy")]
        public bool Eliminar { get; set; }
    }
}