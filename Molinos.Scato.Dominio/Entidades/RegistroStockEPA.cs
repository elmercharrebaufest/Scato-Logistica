using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RegistroStockEPA : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoEstablecimiento { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual decimal PesoNeto { get; set; }
        public virtual bool EPApesoDescontadoTildado { get; set; }
    }
}
