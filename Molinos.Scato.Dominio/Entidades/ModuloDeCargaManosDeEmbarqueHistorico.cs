using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaManosDeEmbarqueHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaHistorico ModuloDeCargaHistorico { get; set; }
        public virtual int Mano { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual ICollection<ModuloDeCargaManosDeEmbarqueDetalleHistorico> ModuloDeCargaManosDeEmbarqueDetalleHistorico { get; set; }

    }
}