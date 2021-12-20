using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaManosDeEmbarque : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual int Mano { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual ICollection<ModuloDeCargaManosDeEmbarqueDetalle> ModuloDeCargaManosDeEmbarqueDetalle { get; set; }
    }
}