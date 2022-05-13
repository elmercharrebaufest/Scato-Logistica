using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogLecturaDeTarjeta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Patente { get; set; }
        public virtual string PatenteLeida { get; set; }
        public virtual int? Tolerancia { get; set; }
        public virtual int CantidadDeDiferencias { get; set; }
        public virtual bool? ExisteOtroCamionEnPlanta { get; set; }
        public virtual bool ReconocimientoExitoso { get; set; }
        public virtual string Tarjeta { get; set; }
    }
}
