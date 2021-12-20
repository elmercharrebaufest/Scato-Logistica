using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class UsuarioDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Usuario_NombreUsuario")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NombreUsuario { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Chofer_Apellido")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Apellido { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Nombre { get; set; }
        [StringLength(80, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Email { get; set; }
        public DateTime? UltimoLogin { get; set; }

        public List<RolDto> RolesAsociados { get; set; }
        public List<CentroDto> CentrosAsociados { get; set; }
        public string Matricula { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoDeQuiebreApertura")]
        public bool AvisoQuiebreApertura { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoDeQuiebreCierre")]
        public bool AvisoQuiebreCierre { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReasignacionDeTarjeta")]
        public bool ReasignacionDeTarjeta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoAutorizarTiempoEnTransito")]
        public bool AvisoAutorizarTiempoEnTransito { get; set; }
        public bool AvisoAutorizarTiempoEnTransitoConfirmado { get; set; }
        public bool AvisoAutorizarTiempoEnTransitoRechazado { get; set; }
        public bool AvisoContingencia { get; set; }
        public bool AvisoEntregaHexano { get; set; }
        public bool AvisoLineUp { get; set; }
        public bool AvisoCambioPinchazosPorCalada { get; set; }
    }
}
