using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ImpEtiquetaPuerto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Vapor { get; set; }
        public virtual string Cargador { get; set; }
        public virtual string Mercaderia { get; set; }
        public virtual string Destino { get; set; }
        public virtual string Kg { get; set; }
        public virtual string NumeroLote { get; set; }
        public virtual string Bodega { get; set; }
        public virtual string Control { get; set; }
        public virtual DateTime? Fecha { get; set; }
        public virtual int Usuario_Id { get; set; }
        public virtual DateTime FechaCreacion { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
