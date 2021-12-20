using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpDeclaracionFosfinaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Provincia { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Ctg { get; set; }
        public DateTime FechaDeCarga { get; set; }
        public string Nombre { get; set; }
        public string Cuit { get; set; }
        public string Establecimiento { get; set; }
        public string DomicilioReal { get; set; }
        public string LocalidadReal { get; set; }
        public string ProvinciaReal { get; set; }
        public string DomicilioLegal { get; set; }
        public string LocalidadLegal { get; set; }
        public string ProvinciaLegal { get; set; }
        public string Telefono { get; set; }
        public string Material { get; set; }
        public string Peso { get; set; }
        public string Observaciones { get; set; }
        public string DomicilioCarga { get; set; }
        public string LocalidadCarga { get; set; }
        public string ProvinciaCarga { get; set; }
        public string NombreTransporte { get; set; }
        public string CuitTransporte { get; set; }
        public string DomicilioTransporte { get; set; }
        public string LocalidadTrasnporte { get; set; }
        public string ProvinciaTransporte { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string KmsARecorrer { get; set; }
        public string NombreChofer { get; set; }
        public string CuitChofer { get; set; }
        public string NombreDestinatario { get; set; }
        public string CuitDestinatario { get; set; }
        public string DomicilioDestinatario { get; set; }
        public string LocalidadDestinatario { get; set; }
        public string ProvinciaDestinatario { get; set; }
        public string DomicilioDestino { get; set; }
        public string LocalidadDestino { get; set; }
        public string ProvinciaDestino { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
    }
}
