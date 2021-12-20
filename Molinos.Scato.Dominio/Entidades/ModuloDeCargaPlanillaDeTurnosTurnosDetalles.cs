using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosDetalles : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaPlanillaDeTurnosTurnos ModuloDeCargaPlanillaDeTurnosTurnos { get; set; }
        public virtual Exportador Exportador { get; set; }
        public virtual string Linea { get; set; }
        public virtual int BodegaParcel { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual string Tk { get; set; }
        public virtual double Temperatura { get; set; }
        public virtual double MedidaInicialCM { get; set; }
        public virtual double MedidaInicialMM { get; set; }
        public virtual double MedidaFinalCM { get; set; }
        public virtual double MedidaFinalMM { get; set; }
        public virtual Destino Destino { get; set; }
        public virtual int Cantidad { get; set; }
    }
}