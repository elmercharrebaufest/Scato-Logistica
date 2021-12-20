using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PlanoDeCargaBodega : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int BodegaParcel { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual string Condicion { get; set; }
        public virtual string SfFull { get; set; }
        public virtual Destino Destino { get; set; }
        public virtual PlanoDeCarga PlanoDeCarga { get; set; }
        public virtual string TanqueDeAbordo { get; set; }
    }
}
