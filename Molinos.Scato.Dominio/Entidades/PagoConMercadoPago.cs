using Molinos.Scato.Dominio.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PagoConMercadoPago : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual string MercadoPagoId { get; set; }
        public virtual decimal MontoCobrado { get; set; }
        public virtual string Estado { get; set; }
        public virtual string DetalleDelEstado { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual bool Devuelto { get; set; }
        public virtual string Token { get; set; }
        public string Idempotencia
        {
            get
            {
                return Recorrido.Id.ToString() +"-"+ Fecha.Ticks;
            }
        }
    }
}