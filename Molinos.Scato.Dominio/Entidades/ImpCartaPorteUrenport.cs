using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpCartaPorteUrenport")]
    public class ImpCartaPorteUrenport : Impresion
    {
        public virtual string FotoRutaDestino { get; set; }
    }
}
