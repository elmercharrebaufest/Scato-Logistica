using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RegistroStockOtrosPuertos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoEstablecimiento { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual decimal PesoNeto { get; set; }
        public virtual CartaPorteOtrosPuertos CartaPorteOtrosPuertos { get; set; }
    }
}
