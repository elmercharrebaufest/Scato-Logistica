using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CartaPorteDerivadoGranario : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NroCTG { get; set; }
        public virtual string Sucursal { get; set; }
        public virtual string NroOrden { get; set; }
        public virtual string RutaFotoCPEDG { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual DateTime? FechaEmision { get; set; }
    }
}
