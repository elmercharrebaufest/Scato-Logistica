using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class HuellaDigital : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual bool Estado { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Acoplado { get; set; }
        [Column("Chofer_Id")]
        public virtual int IdChofer { get; set; }
        [Column("Transportista_Id")]
        public virtual int IdTransportista { get; set; }
        public virtual int PesoTara { get; set; }
        [Column("Balanza_Id")]
        public virtual int IdBalanza { get; set; }
        [Column("Centro_Id")]
        public virtual int IdCentro { get; set; }
        public virtual DateTime FechaHoraPesaje { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Observaciones { get; set; }

    }
}