using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LecturaDeTarjeta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual string Lectura { get; set; }
        public virtual string Patente { get; set; }
        public virtual string PatenteLeida { get; set; }
        public virtual bool OcrActivo { get; set; }
        public virtual bool ReconocimientoExitoso { get; set; }
    }
}
