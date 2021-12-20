using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CargaComercial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Exportador Exportador { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual PlanoDeCarga PlanoDeCarga { get; set; }
    }
}
