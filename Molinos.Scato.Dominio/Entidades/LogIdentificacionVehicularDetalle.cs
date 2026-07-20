using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogIdentificacionVehicularDetalle : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual LogIdentificacionVehicular LogIdentificacionVehicular { get; set; }

        public virtual string ProveedorALPR { get; set; }

        public virtual string CodigoCamara { get; set; }

        public virtual string RutaImagen { get; set; }

        public virtual int Intentos { get; set; }

        public virtual string Patente { get; set; }

        public virtual decimal? Certeza { get; set; }

        public virtual bool Exitoso { get; set; }

        public virtual string Error { get; set; }
    }
}
