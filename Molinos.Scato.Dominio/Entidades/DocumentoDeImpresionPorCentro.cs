using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DocumentoDeImpresionPorCentro : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DocumentoDeImpresion DocumentoDeImpresion { get; set; }
        public virtual FormatoDeImpresion FormatoDeImpresion { get; set; }
        public virtual Impresora Impresora { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
    }
}
