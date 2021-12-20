using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DatosDeWorkflow
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Patente { get; set; }
        public virtual DateTime FechaCalado { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string MaterialDescripcion { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual string Calidad { get; set; }
        public virtual string Humedad { get; set; }
        public virtual ImpResumenHojaDeRuta ImpResumenHojaDeRuta { get; set; }
    }
}
