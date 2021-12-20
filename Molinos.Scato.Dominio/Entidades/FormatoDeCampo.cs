using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class FormatoDeCampo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Campo Campo { get; set; }
        public virtual FormatoDeImpresion FormatoDeImpresion { get; set; }
        public virtual Letra Letra { get; set; }
        public virtual Alineacion Alineacion { get; set; }
        public virtual int Fila { get; set; }
        public virtual int Columna { get; set; }
        public virtual int Tamaño { get; set; }
        public virtual bool Negrita { get; set; }
        public virtual bool Cursiva { get; set; }
        public virtual bool Subrayado { get; set; }
        public virtual bool EsColumna { get; set; }
        public virtual string Texto { get; set; }
        public virtual TipoDeCampo TipoDeCampo { get; set; }
    }
}
