using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpInformeDeRecepcion")]
    public class ImpInformeDeRecepcion : Impresion
    {
        public DateTime Fecha { get; set; }
        public virtual string NumeroInforme { get; set; }
        public virtual string Proveedor { get; set; }
        public virtual string NumeroRemito { get; set; }
        public virtual string NumeroPedido { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual string Estado { get; set; }
        [InverseProperty("ImpInformeDeRecepcion")]
        public virtual ICollection<ImpInformeDeRecepcionItem> ImpInformeDeRecepcionItems { get; set; }
    }
}
