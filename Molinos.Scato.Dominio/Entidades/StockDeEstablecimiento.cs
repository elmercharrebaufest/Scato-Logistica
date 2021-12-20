using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class StockDeEstablecimiento : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoEstablecimiento { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
        public virtual decimal StockDeclarado { get; set; }
        public virtual decimal StockReservado { get; set; }
    }
}
