using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PesosExc : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual int PesoTomado { get; set; }
    }
}
