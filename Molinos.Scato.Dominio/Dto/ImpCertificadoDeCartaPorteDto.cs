using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpCertificadoDeCartaPorteDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string Material { get; set; }
        public string Balanza { get; set; }
        public string Patente { get; set; }
        public string NumeroIngreso { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string NumeroCertificacion { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTara { get; set; }
        public string PesoNeto { get; set; }
        public string PesoNetoOrigen { get; set; }
        public string Diferencia { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string ModeloBalanza { get; set; }
        public string NroSerieBalanza { get; set; }
    }
}
