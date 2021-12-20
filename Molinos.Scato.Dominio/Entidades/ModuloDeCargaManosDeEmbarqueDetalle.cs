using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaManosDeEmbarqueDetalle : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaManosDeEmbarque ModuloDeCargaManosDeEmbarque { get; set; }
        public virtual CeldaManoDeEmbarque CeldaManoDeEmbarque { get; set; }
        public virtual SentidoManoDeEmbarque SentidoManoDeEmbarque { get; set; }
        public virtual int? PorcentajePorMano { get; set; }
        public virtual bool? AperturaPorton { get; set; }
        public virtual bool? MasProduccion { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
    }
}