using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpSolicitudDeAnalisisDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string NumeroAnalisis { get; set; }
        public string Material { get; set; }
        public List<string> CaracteristicaDeCalidad { get; set; }
        public DateTime FechaCalado { get; set; }
        public string NumeroDeOrden { get; set; }
        public string Patente { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public virtual string Codigo { get; set; }
    }
}
