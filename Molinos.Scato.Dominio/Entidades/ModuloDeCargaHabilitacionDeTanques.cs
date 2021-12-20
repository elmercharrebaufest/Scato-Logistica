using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaHabilitacionDeTanques : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual bool Tanque1 { get; set; }
        public virtual bool Tanque2 { get; set; }
        public virtual bool Tanque7 { get; set; }
        public virtual bool Tanque8 { get; set; }
        public virtual bool Tanque9 { get; set; }
        public virtual bool Tanque20 { get; set; }
        public virtual bool Tanque30 { get; set; }
        public virtual bool Tanque31 { get; set; }
        public virtual bool Tanque32 { get; set; }
        public virtual bool Tanque33 { get; set; }
        public virtual bool Tanque34 { get; set; }
        public virtual bool Tanque35 { get; set; }
        public virtual bool Tanque36 { get; set; }
        public virtual bool Tanque37 { get; set; }
        public virtual bool Tanque38 { get; set; }
        public virtual bool Tanque40 { get; set; }
    }
}