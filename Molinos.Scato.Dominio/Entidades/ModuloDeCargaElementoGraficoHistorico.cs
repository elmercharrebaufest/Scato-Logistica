using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaElementoGraficoHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaHistorico ModuloDeCargaHistorico { get; set; }
        public virtual CeldaManoDeEmbarque CeldaManoDeEmbarque { get; set; }
        public virtual string Tipo { get; set; }
        public virtual double X { get; set; }
        public virtual double Y { get; set; }
        public virtual string Forma { get; set; }
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }
        public virtual double RadioX { get; set; }
        public virtual double RadioY { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual bool Rotacion { get; set; }
    }
}