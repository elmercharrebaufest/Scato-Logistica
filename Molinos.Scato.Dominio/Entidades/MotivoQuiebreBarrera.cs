using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MotivoQuiebreBarrera : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoTrabajo { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string Patente { get; set; }
        public virtual bool Apertura { get; set; }
        public virtual Transportista Transportista { get; set; }
        public string FileName { get; set; }
    }
}
