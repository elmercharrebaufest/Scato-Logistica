using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DocumentoBorrado : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual string Patente { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual string Motivo { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string UltimaActividad { get; set; }
        public virtual string EtapaDeBorrado { get; set; }
    }
}
