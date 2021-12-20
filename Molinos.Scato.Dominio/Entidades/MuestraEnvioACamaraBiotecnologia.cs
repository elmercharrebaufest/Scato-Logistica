using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MuestraEnvioACamaraBiotecnologia : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual LoteBiotecnologia LoteBiotecnologia { get; set; }

        public virtual Recorrido Recorrido { get; set; }

    }
}
