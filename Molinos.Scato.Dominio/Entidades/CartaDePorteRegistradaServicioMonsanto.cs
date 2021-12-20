using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CartaDePorteRegistradaServicioMonsanto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual string TipoAnalisis { get; set; }
        public virtual string LaboratorioRazonSocial { get; set; }
        public virtual string LaboratorioCuit { get; set; }
    }
}
