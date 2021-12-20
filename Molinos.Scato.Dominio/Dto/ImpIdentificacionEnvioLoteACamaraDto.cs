using System;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpIdentificacionEnvioLoteACamaraDto
    {
        public int Id { get; set; }
        private string precinto;
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string NumeroDeMuestra { get; set; }
        public DateTime FechaCalado { get; set; }
        public string NumeroDeOrden { get; set; }
        public string Patente { get; set; }
        public string Precinto { get { return String.IsNullOrEmpty(precinto) ? Textos.SN : precinto; } set { precinto = value; } }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
    }
}
