using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MuestraDeInase : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual DateTime FechaMuestra { get; set; }
        public virtual bool MuestraEnviada { get; set; }
        public virtual LoteInase LoteInase { get; set; }
    }
}
