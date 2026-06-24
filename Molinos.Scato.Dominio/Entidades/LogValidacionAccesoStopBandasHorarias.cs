using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogValidacionAccesoStopBandasHorarias : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CTG { get; set; }
        public virtual string Patente { get; set; }
        public virtual DateTime FechaIngreso { get; set; }
        public virtual bool? Permitido { get; set; }
        public virtual string Semaforo { get; set; }
        public virtual string Estado { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual DateTime? BandaHorariaFecha { get; set; }
        public virtual string BandaHorariaHoraDesde { get; set; }
        public virtual string BandaHorariaHoraHasta { get; set; }
        public virtual int Reintentos { get; set; }
    }
}
