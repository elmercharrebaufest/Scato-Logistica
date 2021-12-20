using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class UsuarioQuiebreBarrerasDto
    {

        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Usuario_NombreUsuario")]
        public string NombreUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Apellido")]
        public string Apellido { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AvisoDeQuiebreApertura")]
        public bool AvisoQuiebreApertura { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AvisoDeQuiebreCierre")]
        public bool AvisoQuiebreCierre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ReasignacionDeTarjeta")]
        public bool ReasignacionDeTarjeta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoAutorizarTiempoEnTransito")]
        public bool AvisoAutorizarTiempoEnTransito { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoAutorizarTiempoEnTransitoConfirmado")]
        public bool AvisoAutorizarTiempoEnTransitoConfirmado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoAutorizarTiempoEnTransitoRechazado")]
        public bool AvisoAutorizarTiempoEnTransitoRechazado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoContingencia")]
        public bool AvisoContingencia { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoEntradaDeHexano")]
        public bool AvisoEntregaHexano { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "AvisoLineUp")]
        public bool AvisoLineUp { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "AvisoCambioPinchazosPorCalada")]
        public bool AvisoCambioPinchazosPorCalada { get; set; }
    }
}
