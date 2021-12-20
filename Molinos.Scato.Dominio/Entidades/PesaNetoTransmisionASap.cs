using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("PesaNetoTransmisionASap")]
    public class PesaNetoTransmisionASap : TransmisionASap
    {
        public virtual string NumeroDocumento { get; set; }
        public virtual decimal PesoNeto { get; set; }
        public virtual decimal PesoBruto { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}