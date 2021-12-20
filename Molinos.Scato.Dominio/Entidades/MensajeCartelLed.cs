using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MensajeCartelLed : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual int Orden { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual string Programa { get; set; }
        public virtual string Trama { get; set; }
        public virtual string Variable { get; set; }
        public virtual int SegundosDeEspera { get; set; }
        public virtual string DescripcionFormatoMensaje { get; set; }
        public virtual bool Habilitado { get; set; }
    }
}
