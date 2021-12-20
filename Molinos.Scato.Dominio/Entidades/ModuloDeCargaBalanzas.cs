using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaBalanzas : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual MotivosFallasBalanza MotivosFallasBalanza { get; set; }
        public virtual string Observaciones { get; set; }
    }
}