using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ReasignacionDeTarjeta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual string NroTarjetaRfidAsignada { get; set; }
        public virtual string NroTarjetaRfidNueva { get; set; }
        public virtual string UsuarioNombre { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Etapa { get; set; }
    }
}
