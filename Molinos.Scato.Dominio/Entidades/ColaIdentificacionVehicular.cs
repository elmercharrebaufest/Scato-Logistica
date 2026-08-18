using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ColaIdentificacionVehicular : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public virtual string Patente { get; set; }

        public virtual bool ReconocimientoExitoso { get; set; }

        public virtual string MensajeError { get; set; }

        public virtual byte[] Imagen { get; set; }

        public virtual DateTime FechaEncolado { get; set; }
    }
}
