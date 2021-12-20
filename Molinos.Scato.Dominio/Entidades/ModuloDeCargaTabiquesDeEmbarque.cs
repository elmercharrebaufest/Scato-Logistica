using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaTabiquesDeEmbarque : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual int Tabique { get; set; }
        public virtual int EntreColumna { get; set; }
        public virtual int YColumna { get; set; }
    }
}
