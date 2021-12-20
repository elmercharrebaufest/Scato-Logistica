using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ReglaDeAnalisisObligatorio : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual Material Material { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual DateTime FechaDeVigenciaHasta { get; set; }
        public int CantidadAnalisis { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
