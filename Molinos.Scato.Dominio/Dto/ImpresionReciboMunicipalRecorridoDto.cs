using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ImpresionReciboMunicipalRecorridoDto
    {
        public string DocumentoInternoSap { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Ordenanza { get; set; }
        public string Monto { get; set; }
        public string Patente { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public int RecorridoId { get; set; }
        public string NombreTransportista { get; set; }
        public bool PagoConMercadoPago { get; set; }
        public MedioDePago MedioDePago { get; set; }
        public bool Devuelto { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public MaterialDto Material { get; set; }
        public bool EstaPagadoPorMetodoDigital { get; set; }
    }
}
