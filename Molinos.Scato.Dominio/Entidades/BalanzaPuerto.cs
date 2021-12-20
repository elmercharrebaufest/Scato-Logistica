using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    public class BalanzaPuerto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual int UltimaValidacion { get; set; }

        public virtual string CodigoBalanza { get; set; }

        public virtual string CodigoDispositivo { get; set; }

        public virtual Centro Centro { get; set; }

        public virtual bool Administrativa { get; set; }

        public virtual int OffSetPlc { get; set; }

        public virtual int IntentosValidacion { get; set; }
    }
}
