using System;
using System.ComponentModel.DataAnnotations;


namespace Molinos.Scato.Dominio.Entidades
{
    public class PuntoDeCarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(100)]
        public virtual string Descripcion { get; set; }

        public virtual bool Borrado { get; set; }

        public virtual DateTime FechaCreacion { get; set; }

        public virtual DateTime? FechaModificacion { get; set; }

        [StringLength(50)]
        public virtual string CreadoPor { get; set; }

        [StringLength(50)]
        public virtual string ModificadoPor { get; set; }
    }
}
