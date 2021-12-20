using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Usuario : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string NombreUsuario { get; set; }
        [Required]
        public virtual string Apellido { get; set; }
        [Required]
        public virtual string Nombre { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? UltimoLogin { get; set; }
        [InverseProperty("UsuariosAsociados")]
        public virtual IList<Rol> RolesAsociados { get; set; }
        [InverseProperty("UsuariosAsociados")]
        public virtual IList<Centro> CentrosAsociados { get; set; }
        public virtual string Matricula { get; set; }
        public virtual string Firma { get; set; }
        public virtual bool AvisoQuiebreApertura { get; set; }
        public virtual bool AvisoQuiebreCierre { get; set; }
        public virtual bool ReasignacionDeTarjeta { get; set; }
        public virtual bool AvisoAutorizarTiempoEnTransito { get; set; }
        public virtual byte[] FirmaImagen { get; set; }
        public virtual bool AvisoAutorizarTiempoEnTransitoConfirmado { get; set; }
        public virtual bool AvisoAutorizarTiempoEnTransitoRechazado { get; set; }
        public virtual bool AvisoContingencia { get; set; }
        public virtual bool AvisoLineUp { get; set; }
        public virtual bool AvisoEntregaHexano { get; set; }
        public virtual bool AvisoCambioPinchazosPorCalada { get; set; }
        public virtual bool AvisoPlanoDeCarga { get; set; }
        public virtual bool AvisoModuloDeCarga { get; set; }
    }
}
