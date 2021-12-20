using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PlanoDeCargaAgenteControlPrivadoHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual AgenteControlPrivado AgenteControlPrivado { get; set; }
        public virtual PlanoDeCargaHistorico PlanoDeCargaHistorico { get; set; }
    }
}