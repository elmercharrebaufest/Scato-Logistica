using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CargaComercialHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Exportador Exportador { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual PlanoDeCargaHistorico PlanoDeCargaHistorico { get; set; }
    }
}