using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("LlegadaAdestinosEnRedespachosTransmisionASap")]
    public class LlegadaAdestinosEnRedespachosTransmisionASap : TransmisionASap
    {
        public virtual string Documento { get; set; }
        public virtual string Ejercicio { get; set; }
        public virtual string DocLegal { get; set; }
        public virtual string FechaContab { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}