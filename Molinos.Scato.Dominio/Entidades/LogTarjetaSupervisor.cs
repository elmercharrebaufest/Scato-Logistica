using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogTarjetaSupervisor : IIdentificable
    {
        public virtual int Id { get; private set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroTarjeta { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string NombreUsuario { get; set; }


    }
}
