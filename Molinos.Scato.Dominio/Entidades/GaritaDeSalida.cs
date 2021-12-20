using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class GaritaDeSalida : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CajaId { get; set; }
        public virtual CredencialMercadoPago CredencialMercadoPago { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
    }
}
