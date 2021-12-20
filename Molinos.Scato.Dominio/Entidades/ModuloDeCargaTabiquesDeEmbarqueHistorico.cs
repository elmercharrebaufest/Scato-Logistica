using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaTabiquesDeEmbarqueHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCargaHistorico ModuloDeCargaHistorico { get; set; }
        public virtual int Tabique { get; set; }
        public virtual int EntreColumna { get; set; }
        public virtual int YColumna { get; set; }
    }
}