using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ReasignacionDeTarjetaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroDocumentoIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TarjetaRFIDAsignada")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NroTarjetaRfidAsignada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TarjetaRFIDNueva")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NroTarjetaRfidNueva { get; set; }

        [NotMapped]
        public string UsuarioNombre { get; set; }

        [NotMapped]
        public DateTime Fecha { get; set; }

        public string Motivo { get; set; }

        public string Patente { get; set; }
        public string Etapa { get; set; }
        public Guid InstanceId { get; set; }
    }
}
