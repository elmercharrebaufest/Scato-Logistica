using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpResumenHojaDeRuta")]
    public class ImpResumenHojaDeRuta : Impresion
    {
        [InverseProperty("ImpResumenHojaDeRuta")]
        public virtual ICollection<DatosDeWorkflow> DatosDeWorkflows { get; set; }
        
    }
}
